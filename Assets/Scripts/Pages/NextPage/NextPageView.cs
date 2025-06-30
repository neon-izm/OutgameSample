using ScreenSystem.Page;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class NextPageView : PageViewBase
{
    [SerializeField] private Text _guidText;
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button _returnButton;

    [SerializeField] private Button _soundSettingButton;
    [SerializeField] private Button _addCommentButton;
    [SerializeField] private GameObject _commentPrefab;
    [SerializeField] private Transform _commentParent;
    public IObservable<Unit> OnClickSoundSettingButton { get; private set; }
    public IObservable<Unit> OnClickReturn { get; private set; }

    public IObservable<Unit> OnClickAddCommentButton { get; private set; }
    private void Awake()
    {
        OnClickSoundSettingButton = _soundSettingButton.OnClickAsObservable();
        OnClickReturn = _returnButton.OnClickAsObservable();
        OnClickAddCommentButton = _addCommentButton.OnClickAsObservable();
    }

    public CommentView AddComment(string comment)
    {
        var commentObject = Instantiate(_commentPrefab, _commentParent);
        var commentView = commentObject.GetComponent<CommentView>();
        if (commentView == null)
        {
            Debug.LogError("CommentView component not found on the comment prefab.");
            return null;
        }
        commentView.SetButtonText( comment);
        return commentObject.GetComponent<CommentView>();
    }
    public void SetGuid(int guid)
    {
        _guidText.text = guid.ToString();
    }
    
    
    public void SetView(NextPageModel model)
    {
        _messageText.SetText(model.NextMessage);
    }
}