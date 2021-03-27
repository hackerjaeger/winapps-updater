﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021  Dirk Stolle

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using updater.data;
using updater.versions;

namespace updater.software
{
    /// <summary>
    /// Firefox Developer Edition (i.e. aurora channel)
    /// </summary>
    public class FirefoxAurora : NoPreUpdateProcessSoftware
    {
        /// <summary>
        /// NLog.Logger for FirefoxAurora class
        /// </summary>
        private static readonly NLog.Logger logger = NLog.LogManager.GetLogger(typeof(FirefoxAurora).FullName);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "88.0b3";

        /// <summary>
        /// constructor with language code
        /// </summary>
        /// <param name="langCode">the language code for the Firefox Developer Edition software,
        /// e.g. "de" for German,  "en-GB" for British English, "fr" for French, etc.</param>
        /// <param name="autoGetNewer">whether to automatically get
        /// newer information about the software when calling the info() method</param>
        public FirefoxAurora(string langCode, bool autoGetNewer)
            : base(autoGetNewer)
        {
            if (string.IsNullOrWhiteSpace(langCode))
            {
                logger.Error("The language code must not be null, empty or whitespace!");
                throw new ArgumentNullException("langCode", "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains<string>(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException("langCode", "The string '" + langCode + "' does not represent a valid language code!");
            }
            checksum32Bit = knownChecksums32Bit()[langCode];
            checksum64Bit = knownChecksums64Bit()[langCode];
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums32Bit()
        {
            // These are the checksums for Windows 32 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/88.0b3/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "b9fd5aa3714c8791db354b8e52594d076bf92f61c7e0a552e30034c2bc6341f2efb4a04686b46cd95a28cdd0582b4055ec365add9eea7c2f81f8f4da834062f0" },
                { "af", "70f1642d658205df04aaf3b1b2d4bac59eafda6f57846561a66a7830be0bc95866caeac25a530810fe7950fb08ecccc9a85168c72e8ccd0f3466311c1faf9888" },
                { "an", "e3448b275acbe11d31e982d246969d0705772d6c89712eb8450550a5a1c16242d9de1aa1e0c6920ed1f5ca7e609412f8a3e4cd80778239a22a74226ce7226360" },
                { "ar", "9c6c39f9527c14586c827cb1a45e79fe70bcbd97bc6d8a20fb2984fa411e0641b0778ecc54044777ae272e3e762ab0ff8ea9f8f5321f050c264ab01e8744df55" },
                { "ast", "0bdad436ea54554b1e1e83110e1cfa51a89fec80b955906f3c87e8143f0435ff94014038fc0efeea1a52bcda9615f1dd8d0d195c5002ccc027ffb83f3a299af2" },
                { "az", "f0994cb08f386739d4f171a491cb200620389a1e1ed5be7994b5579f7d5ad5063bc066f1acd20c9c3e56433555770efb2f4d1f931ff5634fca286dcfcc40009f" },
                { "be", "0c92f535953e4e5df7d6b51942ae947da81078394a51002b891c8b7be24f4c5c6b75bc83b068e91ee17d1f1b0126992f9eb6d7ed45950568f0c8da034315d6d0" },
                { "bg", "4bf10c12aebc8c45df2de3f30deee4bf716a92d206a199599d7d367e6145cb0d9e63d161c56e139b800ff0cd21672e6fd3d9e6beab3cfe500d78d7586471576b" },
                { "bn", "ee0ffb8ad9f869d79d706186b38ba33ada1e57205d230f7e6704b2283c386c77c57b33b86e70a329867697e11e44d7dc1190d1b99aa0740f53714933308c0d98" },
                { "br", "83880ecb6c0f086998f57323dc59ca3df8dd397f81a28dff3268c707764cb5da0c454702b5ac1595be41fc9e930d51d51275ed35474d01fc6a597c6ecb8080e9" },
                { "bs", "caf745e4d197fe78cde0e8301b85ef585aa97a9bcf65637973874b73b69e40ea64772932cb67f3e6bc1d17aab58bc2d7232a709d739b03189112f25a9790e0ad" },
                { "ca", "9ad4caf3d7a3263d39e74e63ed019aab6ad2d473f75436e8d78926f4ede6a07ae58108565cca39a206e5b7ac81de9153e8e8f1b5618d7635376164f7f27fba81" },
                { "cak", "b8032be8ac6fa92a5320d9d410a017d4abcbe4cde5f49334056c160cf175a7d4aa0f13214064e622f5990c743ba26d6ce6b08997d844f34b6910d08f143de834" },
                { "cs", "2076fbf0c7ab5894dc5fbebac1898e1af0324defccd17746ca570a0040e5d6c817f206e76053c978e2d2a3cc4951399061d2c1d72677246bec7cacf4e5781759" },
                { "cy", "be9fbdc5dd495cec8070f96a18f5686bed759097942b57652c527b7748efb066eb96bd124388ce8b41af9b91531a8839b1ab36161b8dd6ff60425daf5d03ac1f" },
                { "da", "fbeaa98c8aa854e6b7a7713e6bc0e7753e65a4e1212e8eab1b0420cdfef96eb528d65ad480ca85db9bbaf3a3acb3ef10b504808939dc791cf57fabcb6d94caf1" },
                { "de", "23df4a4e0220e7258d17bfff34d73b4afe7342bd7bacb64871ea2916e5c226fb1eab5911f59eb357254ec8281b5123280ebc5206429dd6c6fb39fab873f47502" },
                { "dsb", "c80ac11566050bfdf0cb316eb26e0020257717eed0b99eedbe7a115b254a32091e14895b9bb8e8475d49db300a83433812620cb518e3f370cea74fc00400b6e6" },
                { "el", "fbb291aef3db98667272e8a40e9a55dbf4e79a865706c9098c08d31d974bae479665d586c09dece1da07d18c45d5329090d7ca3f0c27eec0fa61160118a6d44c" },
                { "en-CA", "50c1eaba80928f8626654ee7e6d45aac0eb56671b60eb4c14c489cb5d4f70bdbfb15ff6f7c484e3af0557c2b5b7b3932e8bb280baf7ea91f0ceee02b5bdd7f68" },
                { "en-GB", "8660bf8eee545c8ec5da47ce508ea66f2eb0d33777e45977446e4daff5fe049ec55cfe55d30faf7dfeb09d4fc4f30d999eb2c2ead6bc33cbddb64ed79f29def1" },
                { "en-US", "654e2c99c093d5a77cb07af544a473e59291e42701de98ae1049fae856ce68f7ff465b72df6f4455adbd11b121de817bc0d5ce39ac3d62efae027213de9a9fda" },
                { "eo", "2e138e4635c7fbd4fc0e56217dcfcbdab4cb683d787ed20a9fb929aeb4211df12d2a5f8472d56b6dfa1837b69b8116d2693168b6067b676b87642d7507ad28dd" },
                { "es-AR", "6c42ad5a5966815cf02774cf294ca5bdde3068691328320f1c411a7d9313d3f1512ac23d5a75d037aec83105eb0f399e0253750a54d1db1bae7c9d191f947fc5" },
                { "es-CL", "51b61e5947df335a9939283c1a6b7f0238261bb9837d977c985bb0060a2c92d6b542baec1d431383c91b864cd996a5c95c52df6c9854595348aff49aa49081df" },
                { "es-ES", "9bbc65c588d4e21ffb0f26377ce577b20159aa941440fa00d490962544ca3e6811f18940438a9f4b6557a9d2e77d50ba053058c2d265a1c822fe33e75409b396" },
                { "es-MX", "1039f0d9e9b9f258dcfca34ff4d608dd557bd14cc24ac96557eea9551f9538ebd630ecdf7c70bff786d16262138460d605b9bb53be0458163932ed9769019632" },
                { "et", "2740baf308d23c962f3e0dd49f62f63a1ca01acb8aa8c09c0ce6a215319006733abe5f0430a4c4c45e76c788e7fec02673265df563e6563909eb57725c5acfaf" },
                { "eu", "948c95fd75f1a8e2be38cb971262a7b3bd51bf737722be4c01b5dfbc7ec3083983f984800c91a372ed5458989eeb7c145b3fe32bdc1e6dbdd56b9ed764be2189" },
                { "fa", "fe685e4b1f1ea5a6c1acbc0ee786bad8b57ee14f5c93d462d2b397fb433493a6e61d0cf15989917f71861eb707e29418ae864d1d77bbef00ab60b4de72632710" },
                { "ff", "d64ad4238a22772a2afd69e8b9fcd462832f408a7603fd268d32157a510ab45ced3d631878f0c74fb3a7a8ea087212c674780e4c03c48daedc97d74c1892038d" },
                { "fi", "cf6f945592f9d0cecb702661fab24e7d0a24d225d8a0e1e8f6b6476a6f3ee5adf1346ba9f7def36cc133f40080d9eecc317ccd53e00d5723bfe3954389e1ef0a" },
                { "fr", "70c22dbb48fb4d18882c4c4621c0f9358f1d717e230280ace4ae80ad0f556d48020cb13b1519e4e81d378cecdaf1ecbb95c9848dd0a05dbbe1a173f16f7128a6" },
                { "fy-NL", "8c2bf0d462a668bd85b3ef4fc0afde7b41624db232b5ad4d77746aa4aa3da1117494206db3e4d82cf7c7f25def0b2b1815f1b4e20c786328aeb57017161d21cb" },
                { "ga-IE", "9636472b8507f38ae5299961a01c7516f458dbeec5fda5b7b67da38ea2b792e74b95acfb400035313cdde7db5376491ea21b339789bf611c2d61cff7991f6b38" },
                { "gd", "a8b03231b400ea8fd94972dc26d29b73a1650bdd88f9df40ec93ec0ce5d3ec87bdd1a8951df611e8798bcaac99a083e29144e5ba10859aae00c28ad22bf80a99" },
                { "gl", "f55e44bc8696d7de6f931ae4e0d67468afd128bc4785a9f873a5109c38aeebbf02367af14b2518857cbf45e8a97ef07b827eea74ae67ce4fb22114ec17e3517b" },
                { "gn", "a02d7cdef8db09e9601e1f6890f94431e2128dff5faf624488c8cd7b2b84e01fb27d68f356812ce52660f742fbe405327c28531d41b44bdbb440a0c7d68f093e" },
                { "gu-IN", "c96cbc6178caa3b7e222354bd9b2f1c971fcc0c584d4a545108c7c01a96034c581ae8872d02541155308fefe68e0892b29ed083e642e73fc51aafda6ed8e1869" },
                { "he", "7d303ce363b8b21c48a7cf7e54fb8824e32dc5185712420277e0856e3e3951365a5a09c8505b85582cec56f9f768aa03a621a72b4924b8c850b18f2bca7db76b" },
                { "hi-IN", "c84788a3ffebf3672cb0c8e385676d8f8078ced5827fad2a3f74ab127d2013045e9d8f5f9d5f328b82cb2dad7dd4c5c0cc012325bc3cc6f255ee566fb8e19e4a" },
                { "hr", "7595c2af40dbd33dadb2813a2c61721354f37d7d46e41309749decb3c55d9ce6b4dceb8b41d49f2020453a8203d8853e5abe2bcfeb6fd26ad8149736f208c3b2" },
                { "hsb", "fe478a28aa52b0e0649de8d394944ab0aaae9fec9b4ee42fdb2226a1fbc7f597dbc1bc7b669d97ac81bf99405e254fcbea950cfb9df4bf03b94342b9d8542d20" },
                { "hu", "c41ba3d0efc41ee1c993a9066aec6bf126fe0696cd8620d72af8fd33eb53797ce21229bfa957e0d959654955d764f1501cc2fbb12907d236e2eb5670bd046c6b" },
                { "hy-AM", "903f6537c5c4d96ca28bcabe523cc168f0d584a59de5a7b256be4f199ef1d3a536f7739a4af4e593782c932926cd93a6d6f227c72ebc273b6baf001eee77ccf3" },
                { "ia", "b26573885d11ef79e941dc1853a6219cab0655f501518d2f5b395b02baeb90ee0df906584634cfd89c0b30df66388a0d2868f214615a625d74f54217bcaae9a7" },
                { "id", "4ad46eaa7c61314a2ea50e3b5c24d87d0327a2fbc70095f235a58a8ccd362e6a50836b394d75c77aeffff692e58d986ea314964fd5127bdb1f6e29796eeeed98" },
                { "is", "8059e049cda39f8be8774772a45268f0f4c5f68c45efaba31e894923901666a62e324b134f4a7320ed69d507c917b01e7e850715114bc134a1a6aad1f5e8ced7" },
                { "it", "819a9b503c8e78da7de2e0de8f93420dee76088381f2ea5656283de2528e57eac21d7958e6a773599b3235ee04fb0f00f16572595666268b46a1ced0b29678b5" },
                { "ja", "8b21518d39d060bb1ec7fbd32fa6b833937a2ec8880534da1a252747d41cdf8741330a45b008e4839fefcf4b5341f5276d4a212662f0890aaf57b08dfe988675" },
                { "ka", "aea62f5d1a23ab33e6dc448d03d6f3848e0645d0409f21e08f24d062bbf3565bcb4f4a05b74710868bfa2e6d6fef42e344ff2bc9d8367fe5702b771f12485df0" },
                { "kab", "d031b3a4d44108c23a103fd7595ae0ed49b4d9e4259ae2186a5659054dbd9d193beb6f769fb9c1712e8e07bc26d17918be670db79a9ac3ea03265ee13182d3e4" },
                { "kk", "4ac95d1bfc7f00d6c5ebeaedec9c6f87f39545b27018ecdf618f1da20f95a60cc8109d05c4923ead74f59c4d43d3ff25207bd403a1819a18fbefef3fa8d4516c" },
                { "km", "20612126c8586342d553ac47a4cc7be14159da4a5cef3d2cce3a614cad9003bdd3f88e5465d4af675dbf7c6f2d3498c6bd5dc7a25cf4895fe0b9921d72d5bcc6" },
                { "kn", "c771f65f988bb5e27e61702aeec00abdb48dac2acb52cd35a56c8a01164a363cb64c17b1375dc96ccb2b0912cbc805378adcb389a5bed552ffada9ba9fa88a5a" },
                { "ko", "3edc43e432e89dbf600c3a8f59aa448ce14f6f66104cc8da5701c6a2c29e2e6d5dc9bdc76be64007bbb68e13d925e410d28c34ba3b17162b986126eba587ad83" },
                { "lij", "c5385c131b105379516f14e4a38e7ef6fc3cfff2858bed4e9dbb3cfe329bda891b40955c3243bc74389ab33bfc093bee422503a7a633be128e92cbdaadab8b8c" },
                { "lt", "472a8859d63a1f5bf827dd8133bd63e1ecca4aed902148b8fc44fb277db19d862b6af6f2dabdc09df994b98d04f3a3fb8b810a96b1b082d6e4b9729639e66175" },
                { "lv", "8115a9f34ab1147611c7444d4b26fbbf6208b12ce9aed982b0540b931a2912e139b87ee9ea56fa99bb6e884daf661500468e3d1fd8990e87e693de5f931644ff" },
                { "mk", "74b6abfc1cad7b820ffacbf215100be07335737cde61abbc84cf17b51aa1bed57ae7d3c8eadd5083c9ddead1c2bf46db77559660ad388cece84f5cfde9ab83c7" },
                { "mr", "3d544df6d09c4e3bcefc3fe8d21698dfcd008639725c56b16d23f7e9aaad9b1fef97f58c4d751aa5d338cee0fc4f3b6c37aa79dd674610af9fc0208dcd23534a" },
                { "ms", "521346a2b68472ba71a978dae54990b3950a4b1f11544fbec8157b92b147a37a649f0ae74a7fcce1604920dc164b4c0a553b27ec98132a8a1953f431f2989328" },
                { "my", "7b49c5351ca954d270e826262f0e7f5a4f4fadd5ce4a14296e04d9fa468c237f988f7db3be6ac845890afdc7efe37339d247b30d8f4399d615dc4ee2e513c566" },
                { "nb-NO", "6e382af0ae00abb90b73f82c2eca2ccc044c8da603e79693b01dd3ef7da238fd27f4ad6e8f953c26d5fda8d364f20aa9c6fb676a97aa235aa1f7e9805801fe05" },
                { "ne-NP", "857765911431fdf0728b5442a66b49f3033bc978321a9196ae5af2f996fa2a517b893c529728aca5a9f7e82aeb24d61974f259523b3eaba1369eadc9e7a2607a" },
                { "nl", "fb44519f5a43a70427e42e88d22b02d6bb562fc10bca43ec2553452b78c2108b2cd5a3fa95f16593d66e9aeaa5f3e966fa54ff1b400d7c96738c6e447dcdd28f" },
                { "nn-NO", "7d311d5cb758a1d8c063e1b01f513af2d9d95409a0bd141a6c5e42f1cf1065817cca69be8ba596b8abedc0ab85e092a6cb327f070556c9c3d77f0dbde3f24898" },
                { "oc", "a8ad890d7759da490334eca8b202af06c0e94113b49d3f05b53ff5c38d6caeab04cc4a13a1c97f50d10c2e7e9b540c51e445e30a3d528c7c36834fb29ed2e884" },
                { "pa-IN", "0fa38ba914f327f72370f9895e82bb3950ee751c30af177464f6f336dd332cde1d7166d33d5ef85f6c4736f8e9f3b9b5877ce54ce2e451246f02803ab7503d7e" },
                { "pl", "4889f90f6ee4206ec87fab733d25a624ea9c556c4a5de68e0418fb04b43df0f1fdcadee668e4b3182d4418f80eebbc5ad6e0fc204b7b48464db759be8a36da44" },
                { "pt-BR", "284ea15f8158c8e26c965520dfe685bd0f5fd3eb8c3e6be1a22c313288ddd02bca3219e2ec096f09bef583ea2849254249bedf0d4e02f2389f4ed5abf5365a3a" },
                { "pt-PT", "be3e00f3602836e733c7010468ac6a3a6d273a7323f437cd1e7984f42efb1253129e098d74d61fc86c5c88f6465834c74fa26146041f1386940e3578f0a22db0" },
                { "rm", "009d56b2ac35078e3ce366d954a64fc53d20698a4af0868b6bf35664a18840278a5433cf9328e268572660f3a8cbde4e2552131d5f1475e6acf03d97386b2d18" },
                { "ro", "b918cbba1668f34e9847d37993da4490e07d99c98fa97695cfaa5e203fe38b9b9ca76c6c1eb148fcafcec62b3d1de0107c2b5d1b9e7cef6b061af0268642a067" },
                { "ru", "d7151734bcc0c5306acb96412ea829a4cc0244c1c153551974e2e4ad173fa081ab805be0fb110d2a4a5f9b454a7df304d8cd9f86ea7a2fbb846e14ef325d0d18" },
                { "si", "579ff8a791599e85e4cf01cebbf448449192ab456f13d6b231c4a1e5e01186de6da1efaae6f2830bb508f8ca6eb65498811e83a63bedb93405e1b020bcd779d4" },
                { "sk", "d09ff3454cb60212d07756851a4506ad2181ce291a361a8f557fdf43cdc6472bf4404396fa5cee1edef80ff197e5ddbadcde5b48941fde7a492dd9ba48b0f8cc" },
                { "sl", "6820fefae910dd04330e204514c6cc0d54802055710f7a96e2ba62852c1bd9227cca6e227694ef87627ab116d67ba851fef547bb0db8d5ebeb0b83be0f622f18" },
                { "son", "ad1f164151d095e2905b7f47fcbe2927a968d376048e1874843570ace77ebc99183d78fb25d85aef70fee5b2f125f44609e128e3497df90fafb23b253036da06" },
                { "sq", "d45b55e400c98d0de20f16cd2a8f45c112d002217f075defbb99ea8dcd41f801c46ba5a16430b436ea60d55b9ae182262b5f920d06b49ee93e76510010c58cad" },
                { "sr", "f3c00a00acd21ddca678e1b0e4f5bee78b9eb81f5958d8bea29889b08140e1a584df9077377ce2a3687a755354df03df14784e469783a09412376ee1e48bc359" },
                { "sv-SE", "17871c4e35b20ff05c53bf1d8cc83664a1ac6c92830dea9a73fc64b11ea1e0623cd468cdc3969a5cf470ce8232af5a697c50109eded3f0069b5101ec5f2906c7" },
                { "szl", "4b8131056fe4ec0a9926236fa7d5a364ffbc9a782d8b12c5c8ea11ec077be7d46eac720b08a7e1d556c8110865b1f85601ac6b5f69f92a8282447515e4cb0d74" },
                { "ta", "064ff9b4795239268a882bb19358c19291b8b21df78279bf2e3b38db12aabe6b68bcc409203ecb1ba723331831dd412cb9bb271e5ab621c86af4f501768596c2" },
                { "te", "a74981520dd8ab5b0bfe66a0731ea613a7c224df42e18f3868d73140a01e647a8e6ecc8001fa6c532edf60b389be32169050c92a9d237bf314694e5930d2d665" },
                { "th", "7bea8bd039d2445d37960ff269b325a67d73dfba467b7e97ef14ae8190c9469692a2975d7074e1072b2df0ab356f62307833b90087b1aaebe7fb082af628dc31" },
                { "tl", "65c7cf47944ad73a0afa62abee27c6416b2a423d13ee1fc4da418e101197a9fbcb372b6906121551499fa9568d8deb74a094fe72d73ae1146a51c09a3024f25b" },
                { "tr", "d9e26c5a9402db6a09258ed1221a09ec20a17378a4704612cd4586ecb47af9708ee2b93fe385050b497ea9476d4f7ccc43dbb7687a2267d06b70c7ad4c72ad71" },
                { "trs", "7dfff59c29d82bb3f4f351c6d0458f9b5819691f9b9ccb23a2e4b0ae51da922612fc128a8add787deac3313c60cc4bbfac5bd98dbeb7aad6c493aa120ee2f69b" },
                { "uk", "493ffc92ec0b9c3f532d740d5c7fc3dc21e2b1385f0c6c9289db5a1fe6802e4f850902b8ce0f1cd98c28e159fbb2130284b4ef1fb94687aeb0c43e77113452a7" },
                { "ur", "e7a04e29aee58962fde813bf693829bf75b1930788e74b9f38c33e17bf1a785d3ddff911a881068b271d01ba2e385236da0be2379cb753d09b986f3dc248f1d4" },
                { "uz", "40514b5bf65225abab197d4c5f35ca4690095ec4b20a4cd932b5efddfcce8e56eb6eb9a8acd1ccac8f26611c332bbd9a098d7cee8e02118eb479d84f29eef4c1" },
                { "vi", "28ee5883e6f0ea01f0791bc0c8ce36789c7dab7b6f63836df7dac057b5b7dcf79bc9cad24991501d50cda245cc4a2ccb6107660d69c630c2019d735164429972" },
                { "xh", "1e70789bbe08d69650417d0986cdefc5fc28b03e25797e74ee5d0c06e2840cbcb342c11cecfe7782dd43ba475728e4c3b78bea1af4af0a30495464ab78bc8599" },
                { "zh-CN", "00b218e23773cf6d8e30944a85318baa636b1107cd37e32f75a4e9504e56ee6e6bc189a3a3046319d359f8f6750945f0a015522fa006b111dfeeac3e4e3cbb95" },
                { "zh-TW", "438d13dadde88e708242b90c7b43a0a23501a0929d2928891cf0624ba3355451f96ce49e5e09ab13b62899374e5f0bfa16e0802292cb38beaa9dc1c0ca8efb8c" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/88.0b3/SHA512SUMS
            return new Dictionary<string, string>(96)
            {
                { "ach", "2d6f436eb7db4756d14aec996a9e0775fdec88319490056dcf578b8985a16bad1bfe1ff33828564b2c698f623c3f1aa8697b7b1cfb7856b8256793935fd87e9b" },
                { "af", "cc0cd4f849c86d8a4a6bca6d848d9f2a874d94a23fd336a9dcbd6d471237e3a64e698110da6164d0421ff38c2686b30d7363ca7b1b914b8e95665a657e2a2d1d" },
                { "an", "1814d72ae31b4bcec491d11ed83940fbc1394997ca1a61740edba73edd41c24e32a303b08b7a4ab0c87776604c4ae868201345fb1befec21c18759f815f463fd" },
                { "ar", "c5728010b7824ac82a2ae4559fadc30ca5492fdc6ee668640d8a7edd1ffb2f054c78b727cd5e827c8bb06b090abb2f429d2041d472ed2458a59d7ffe4440c19a" },
                { "ast", "1e1bfca19c6f2e62330d971017c50899a4ab9cef0ec665837b1240734483cddaa862be2d0ae4f9d5db0774c06ad51401e89828f8cdabee5ffdd30208165cf54c" },
                { "az", "15aa057fdc0601756d571e0728289ae058e29b55126235b883d11521664241f4cd6a4463a3a79a2ca4c2d46674b246e489f67705161f77e2b657d7db12bf8040" },
                { "be", "1b4a8119a1c15158f12ddcb4a26f1c46da99198be5d675be65ac5a1b6069131df9b22ac92532e227a3cd67e5e7862b606828526f64744846eaabf65d5801eee3" },
                { "bg", "567438a327e8fac1b4756b41271bd5f2a7cdc05ef4af04301c7cd4358208f631d1aca4977627d159cea8a2a7951a344b2ba9a67a7ef0d5f51fa5c698fd917687" },
                { "bn", "9889db4277bb8c29e62661161d9e9d8f842e8992dc8fd56fe14dd0009532b6ab17d955b5d9f7fca5c8da12eba3010e9997059c9d057ac687845aedda45010dc4" },
                { "br", "81a5fb29e3f7e1a0ac9dce98011c88dfd7d9c0fcef2f047cf6d46c19ae1e0097e471b78a567a2d231e4a02d7a3e75456e967236917aeebb735aec0be42a6b4e9" },
                { "bs", "08fcf15b5bf4aecac29d2256f373af43fe4054c70400c1cd36a33f4d5f7a060be7d2ac06208f661b9a50114f0cc666c1b0933d9c0299c6c09d0c6af026949220" },
                { "ca", "6efa8780a875a5eccde2be2f730b53833972b02c60d1a06499a656fcd1d123bf6c556fce3a7c06f56951c704ec45987a83e6a01d699344ccfb228f0449d9b13f" },
                { "cak", "1818f89b0fd1358d3949dae19b6e6308354229fba4aee24f78e85e20f7d0fd228e41f61a7b1b98efed37544969768c4dcef5746d86c9be7fff05fe8b39330044" },
                { "cs", "45599ce0bba01859ae71bf07df5effaa188302e7aa6d0fa122176bb7cb36eff5a53736afff69411f5ec5f404df010fb791f8a6a9740cf36fc7086712307b5bbe" },
                { "cy", "ba4b033a208780d9a240de264b83f1bda7e460f15b7f606e26aad7f95a7fc7091b0f2b9aa2beff27aab93ee093133fae2ee9cf9068b23ad2188ca5b69cd320c2" },
                { "da", "4af1a3b9c2bb3c8b0d371d0d5f24ea38181eaf7f11fe429ba8e11242538c0b1fc1086d97e3c21ba89c947ab6f02aa7a4cd22248e78046a9a67917a38f9e516d8" },
                { "de", "b0e5f7baa79dbe8aa18c932dc9d187735fc667279d5afa16d46d8023a7ae6c17158393d7acb8637297ce728d06c7646c05b70fcdecf17e4a368c848caeb01851" },
                { "dsb", "4057f70a3ea1e1ed0dcfed4b2310ca626da834d331ad6c5fabc02cd9a0a9d123d17bba9b8ee5287002d23d81193c4b3c90308693cbe88f7973d26f12c238a5c9" },
                { "el", "b2475353420074257c1ade1464ade65cb7ae324edcb0dd9cc444cd2a71875e9cc2740f870220826b09ad9b3be1623f76ae94ab5512fc2f8e77b0134030865969" },
                { "en-CA", "5b6af8272dcbd998d1edcd11b646aeb8b8677534e9a906c89e24d073ce0f3ce2fa905f10aace64c83b69ec8ec12e2a2c19a8668a0adac279067b8c14f8d5897d" },
                { "en-GB", "ecb040443e314a818b16e3e629be1f91b6913a9bd1c0fcf5af2098e6965cef28bc2e65a81e1738aee87f816d5ae42adf504662006f1a37a72ad4b8c27b5ce8a5" },
                { "en-US", "a879dd2b6bb81041bdfa1969d0791b9770994b2959342cdf7115d6da580ea1fa14395f7b4de5246ccdab7051d25dbb85c442118c0bce7f9257fc0d80c64afc9a" },
                { "eo", "c815ee795ccadce1500e5eff21842f028deeff2081a6b7cfb97059a83a3164ebdbc9fa5ee525fa972ff033fbe0d8f1da4dc41e4e48a2f8fae553b93aff9ce4e5" },
                { "es-AR", "06374935e0fc391271aac48bf1797b6268f8f05ce5ea652f4abf27227e50bb55892661600012cdcc20be493e4e65e7bcb9bc0da0a1b094d44e6badea4d5e5831" },
                { "es-CL", "38a7349511677f1089ecf4773a578a4543456195364f65030ec6c36f43df6ff0376e2492d427d8aa6cf376b8a5b5b08b5f3b193f0bfac167703f34d0cd3a7ebf" },
                { "es-ES", "1b035ca810897bbb13ac51fc94b3486e1fff5bec67cb4f92c6b7a84d0989480ecac8c94ef812122de25c949541e50d71e434871c857cadfa1fed107dfd0df5d4" },
                { "es-MX", "c5ce36f79e94ccd0fa71f8936ea164b4e8d86d747bc8e057c7effe80161608b7606f4d7eee3844d61efb1112eacc43d9f222610c2a9719e402c607aa215cdf26" },
                { "et", "9e9dcd9d5c97fbf3d7e7eef29a278b9839288933268dc0fefbe3cd92336a2ff5ad4937e49fcafcc0d34ac9d9bef1cb7fac3377fa2723010c6ea7b1201eb28985" },
                { "eu", "6d1ba06eafb5279b74fb02e5cf7cb5156024b7d7aad1e54c6a410b353ddef0953909b62ae3b8489b4ddcb98a05fac8d20395414626ca785b30f6d2bd9278cbea" },
                { "fa", "69185c1018c90236e08354913f92d5eb1a37d914a232349114b97deb4bf10a9b369b24897cfb6308c6e12a95d36b5b3ce7585d6c9c3c6b39d033ea6279494b46" },
                { "ff", "2d9298c8e5d46b26ebe9cc7604141d2e735673558dd20184893ea41203206ecbfc4e6c3755b142309f881c99deb8232f0014faa5684538bc3ac8ef18ef422687" },
                { "fi", "4a1711b0e25ac537db4c5dd6f1670fbc8720ecce31f188bb30c90c14ed617064e925d5d74ef5517916961bd16ba79950616d5608fc67e5ff5f78ccccde57fbf0" },
                { "fr", "69e62b1f459331dfd05ee04a049b89cd0a8cc0c8f381f0a06929c75af7e9250b5f5b31a91cb33e0e2b8db7592d23f41206f16ae03174d2aaa0cd7fd1d529071e" },
                { "fy-NL", "9f2d4c6f75b1a1a1c8d7c5475800c59228f51d448a5c12ecfa261acebc0f956664c72f0592f371f1cfb77649d6d41b51282b8ee092ae036584c02e1c8cb4e6a6" },
                { "ga-IE", "6e35e1b48e642e27eeedbcc67fdbcbe4af562627f3c85108aebac509dd91e6a82cdbba5c52994e5f21c49f4ff2329b20858ff3ecfe677f3f6106cca24de6e9b2" },
                { "gd", "83018446b70f23dfab86e5f319f24db60cfc551d9c26452780feb217d864db6553bd50e7167d1905ed35b9a698dc111ed027691f29b205ed259bc0208f9a8c30" },
                { "gl", "43647b814c699b0378242882157cc1873b1b6e10afb77bb598bc6bc5791a9a43d5305834e3e7e85e8ca2e5b85cc6f59c2db29c27f25d98510ab8a06dae8b0fcd" },
                { "gn", "c0c6cf9179e766324a8d83d8926ef393e4f4ec8c39537ae13d53e6137278ad87bf56be37f31299e0274bf2955515ce4380ec4fb423c9ab81e54b90a79cc9716b" },
                { "gu-IN", "4b0dc9ec6ea9335bce8f193e87f1db5ca88a9d7e6365c6f22a38b5bd1db211d4fdeaa5e6d4f3bdebc589dbdda8bbb588a1d4472bf350c65dac8fed7d99c39129" },
                { "he", "f361bdb00b3755a5b3df0d5a244f8292420a6eebe08c31d86607ced0bc682eec47899c4099ec85785a9d33e7f55204cc90fde39af3f263db5e919d720dbbf05c" },
                { "hi-IN", "6c84d5fe7327a4101e886dc000802d14b0aa7611ee29c8874a561fac49666f0b962bfb96afc6b6bb01553892c372eb3a1c50d6b9bae7b9614800c5e77e5903fb" },
                { "hr", "ad415ea738acbaffde8aedbfac39f0b87e793f2c7b4d823dc0879fa3c077fb853109f455670913cc25583e9070882f7c73be6f035ac34a61a67e8ccafae5db09" },
                { "hsb", "2c8e3c8c67d23bb106ed09dedc54d8c6bd383fec40151ff5a597da0bdc84cba0846996b412f92b9060e20695bb0a10f2e1632546a3d87f337af7b3d7bf089f48" },
                { "hu", "eb2d1a2a293cec7bf82258fd9aba8d56a8cb8a1c64955880da2b0bab79c771ae009d0dd910681c6dfe415c1b3194b91acc702c2f664e9aa3bd63d800d6e2faf4" },
                { "hy-AM", "6e8109b2d056b96d8460e287185433d6ea7807ddbdcf6c7a704186a5be17f9fba49f7da6a34ab6c7eeb5eef0fd549e23723039c6f3bbb354f5e47f8cb679db4a" },
                { "ia", "014c06a35e6634a09409ecf6ccd0052386a01b49b74efa18e25063bc48e50c4f5388a0cdb8315107fb2e5e95f0749bed5d11542948ccb14e8ad99adb2f108b88" },
                { "id", "15dbb1a14442150827a17ff02e7210d72b2c64f1742da7f5b1228788055d599e8f6b509d4df76abb98af51f598256365f4a40a83a48db92a1190a9ab2afe7f44" },
                { "is", "9cb3beaf4f1bfe1a97b6ee869d96ca57e00e071fe86d8c95ecfda7bc2000c42afaf0c87bbc99ae85d31320839dcc4630737d53a65425c95e2942755b28af03b6" },
                { "it", "b2b10cf0b6ab1e8a52ed3fce5a7c3b308191e125f5ea8a405210be191f239f704440e3c18e554b7ac4de05603e741c3af5889196cfdb0e38b7161ee2fce3d7d3" },
                { "ja", "a4b80099f6aad4553bf7b3e57a6ebd128b4cbea8f76daee1b649b2566a1ba6cf6f307eb5e9b799136f8f7336c062c597ce0a32a9f511f3be0315af7696521fb3" },
                { "ka", "6b48a27a5fd3f8479c74071a6b3be42faa896f400d519bf5e4a827cdda9099b9fc731117b56538a523b7f9dab34963cbbee3b65a20100bd6e6f69f0df9f97bd8" },
                { "kab", "06d05f218fefcf75c1393713b96a949084bcf21d08e0e7c21bdbc64643468e9fb5294cf2d10d917ff894922bad666b411718b834060259686dd4764556e752e8" },
                { "kk", "3b3f09ce7bfca9f81ed8494b3f7d3c3256d432af5f5f021a143c9693b78c286ceb0c1a4d588dcd1fb7e512c0ec41e28e6ed63b5beb1466b1742202476594d9ef" },
                { "km", "8357bb9d1ef0b35d202aea4efce182c3a2e120371a12c8de7dd6a4caba47753e0b3e13844298dc4d707e8f0f0864083f86159625a815ab0275a30d487b8157d0" },
                { "kn", "abdb4b29ff406bdf23dd49ea1da9c2741644ff65e0e6310d8cfd22aa5e230f7e2c50620e2d1c613986e510ad7289fb5edd3cbd35ecac0d9bd81dd97144cad982" },
                { "ko", "73a315f59de38c13bd90066d42c1d752c66a9cb739a27b77111ecf40aa57676079246106317a3d650928bfec331316f5465fd9525bf574b8d8345119026ab2b3" },
                { "lij", "330f046e7d4beb1d1a039d2850dcdde140e41010a5446328ecde68530851d2ef5570a315d103824bb6ab46419b5d46e9cd0c8b464ccad59bd7d3057a8577c99f" },
                { "lt", "e5b328ece134bc501703fc78eae63638e22f17c27bfd0f35b5d47e391537cabc62a859d338da4ccdea7a4db579fb7f7b94993e10ab765441c6f9a23c02ab83fc" },
                { "lv", "2ffe1a61d7166bbe08f01b1236301fcc0ba37aa24b1a125e46448d8b6339c21b572bb9e7bcb6ae39b81e000070db63988cb23f832c47757b7661b7928185f2a7" },
                { "mk", "e649a40e97b0d7da98b856e69e729aebe5b84761866097caf041ddaefb628afb47cbabb30bfdaaa77facb03c74f4c7d0e446d033815f128041a9e72614807f32" },
                { "mr", "acd301a244ec8f745d19842b910e2d160932191f270ff1eca325083ee74c33e11d702941cce5aa6a496955bf220b15f7afb3e89c257aa4147ad473b93b08c213" },
                { "ms", "6c96b7d7bdb60f081b8017203072b29f9602360641fef8593c0bfd93a4828a1ce941339dc251d81fb9da0869f9f746bb0377869a50c8a805de8497edc53ddde3" },
                { "my", "2305b4f0a704a190f60db982915d076f051074cadd066edd40be6170b69beb5775f6b3d749764094ad601caef0b78351c0aafe354f44630a2be7686ce10af5b2" },
                { "nb-NO", "842b8d90971111645135794c7837459c6486a8b6a59bd37bd45e19dc076b8e1070c3d8cd4b1e32ff951dc4f294d39a46668efce64ab9ca0321725b78b9d5565a" },
                { "ne-NP", "3f263a95debfc09d5a21af05b2c57870edb590bee85aa26db04e3a2259ad8def15571576a6adbf655a346d12d478befb52efe9352506967a091bfef1960ff805" },
                { "nl", "47f19754184781392da7b8237a9276ed69325e835c89a66aa46d0b7bc4e8f2e36acc5c4603421e0530157462f0fabbe73a8f727d047e2b15e414f1ef2a72b19e" },
                { "nn-NO", "a278f6f1192cc126e7d0629f3bd0a2f1fe362fcb1879afd7fe12703049ed0959ce9946e5e77cec305c077cc558e89fb665041a8e01d08d37fe97b67eb1ebaceb" },
                { "oc", "09f25cb3950ec1a7eb2cadf5c076671ae08acd74a6bd2997f6a510c82e2f1bd37aba2cf44f936aa5a43673b79c4cbe09d646e35dc11534f811ff3113da638ea3" },
                { "pa-IN", "8b658eb742b87e35337a1df3d0c1b1735ce6a7417cad46b8c741f926ffafa2d67e571ded2f6fe5bd45a156b5d97217b2a0632ae25a06ea918c4150e7c2c4c9aa" },
                { "pl", "fb2c7bf9ebb895b4b5bdc7e0980e1bb6f5fe32b09a7985458cdaab13903ee507ca0bb3d2aa5188821ab32ea0a59130735c571c36daed16d40f1b91486616ab25" },
                { "pt-BR", "83146cc3f607af6e999d35b79384463cf81b6f67860d624d835e85a0b811fb1f78d31663815e6f56ed297db493098c9934abfa730d7ce711f4479def4d00d86a" },
                { "pt-PT", "b330e9943c5e9a26c7c845e89cb873616669b91857546a6fc9f26a64500b0127c9869bb844470b4e3abfc0348143a782a2308ad6b8519bdb4e3193603d0bcea4" },
                { "rm", "778a2de8bd4e695e977c82584e43525533d245c39a2976308a98ce59afa2cbe1011af64a5f67dff8f1e6b09ce03b27f36b4107c6c317a6179c7a5e0d9145f511" },
                { "ro", "98caeb3c5143526b0314b8b177e61c3a0b8752168cffcb72313fb5abf233188631e46cf7c1275af499b5b1b980e180c6c35edef6fbf0c36df7fd3efc60b39704" },
                { "ru", "179d46266bc39ccb3def76c9ca0629dce288cc3b8f1cc6c0583b9592db3328bfe5f32b16984b170cde3d2193531035810d8a5d48cb3f09fee419a0425440ddd3" },
                { "si", "1ad73760fff5ac51c863c4c3623fd9ccfa13d51f842793bb0fc875923bd964a32f6a6e592e5f3353fa8f703bd728c58134965f39e4cf698d0fb2b8020e48a715" },
                { "sk", "f3cd1bbb7aeae7139c3a97a80302893dc892bc2320a2d8fe75d0068ef95a8bab4c3429db900324e91e71ca0acf22fc4bb1fb2db4b6e2f08acdf6da69d54ec7ff" },
                { "sl", "294fd4b9f3eededbf58fa2c037fa06675abf8c3621623d3d8be8564fd5c56aea239258de84482b8216e652b98e193461dd8d51cac870e65c3fc02a88ec2d6bce" },
                { "son", "acc7de22e6f3dc52ae5725ce8d9d3f13f1d5466c5f564249db2715a487e86c181f2c896ae761e3c198c9ed007060087f1b9b077b9684df0ebc01a29228c18f53" },
                { "sq", "1990c2c679acebdf8d3555430214b4d51896dec9c9bf756faef830c5662074bcf03755fba01861f24a273dc36c206973bc0fcee753a4af27f82f9532d5d2e239" },
                { "sr", "810a645af0cc2f30dd155c7271731b78c4217c5fcdb03d52b8cdc6749fbea1f288d2af830e4d95d23f7efe6975e0f779b70282054a370b8b1a5989ab3a507b7e" },
                { "sv-SE", "01efad0f0fdd4879a5cee682c9d7b47fee6dc784550bd1aba2eef2956bf27b3bc83f4a05fea1b2e81fc35c91fbc2423b276f196db70c508e86af0c77e330c9e4" },
                { "szl", "3f6668b0bf7b09600f37872d4c091f5c2e89337d1a660a96febe03283d3e4effbb7ef40c956957b0dd82b639b381a84b24d6c9030765ac7af0cb7a28bf85976b" },
                { "ta", "1d8c3c7493fce55430327519150e2f3204609ea12ea750714e46e6231cdd3c053d43ca04ea7efebac1f2dca9648b2b8bd4d80ac3a13da2317329cdfb48b14ea6" },
                { "te", "189d1cddc90138551b7ba6af39bd829b06a0de5b76c14acd456b919cdb785ad19985e6795dd30768b1703b4c5002105b5f57f42c761edc6804a70fc47580b5e6" },
                { "th", "69b938da953249cd0141638c3c396c3dd9f133372d6992142971b5e7dc87ed50bff5e528fdd2438fb075729e6b51517877b236e978017ed0268722e3ab9a97b4" },
                { "tl", "6437a985c19a4cf16d607f6f95bc0dce7577bd2e94b070d3695fae0a9dfa4066fec1f5c8840987f0d5975db26ec029e1520d6afeaaa1ab2a79ebe550b4b196a5" },
                { "tr", "c60dde0ba17b93e1bb6cd2931878666eb6c1584d89530e13ec7a3af1e17ffc6e0f99ec792d661964481412f81aba77bfe6862ac8349282c7c3f34629a4dc1909" },
                { "trs", "2505f1328aff8a291a89292ac8c4b23c4fa2a018456901e556358895abf9cecd5a9891364926b0a828b54f5eceaa05febf24db19e57d7a6a495596d8f550870a" },
                { "uk", "6502d07d52392070f1bf3aa652b7821fe512dd3ea6cb255de1bb73e7d632d1d48d64ed83ead1a095bbfd97e6424e2af3a8ad9d5fdc4c07d70917968cf4326eb5" },
                { "ur", "45c466d87e91e28588f9ef6856f12359287f79f565aab6864ae151f8b0260f01b86fe81ec0e2065c04e2be379f3457a6ee5c2268396e6933e2a05dc73eb6ca8a" },
                { "uz", "456488b585f81e78f82f6431835f91d75a7fc1b072f68b4a8ceb0e0b46df5491f639fb45e1edf85bbae8b1cee14e4ac4586a11816f44685cda7d8705f6287873" },
                { "vi", "f0d1aaab1a994ef1eb753ef17f7aed506e945cd190072e582594ee80a41a9b5e9ad03575e13237ebf290f3a502691f0a035e1ab9743086e8b2c409f8a2c864d0" },
                { "xh", "50d679535b245f515bf03e8de988dac9d36c8bc9b6a8389dc1cca3f33be6f204d539bae868b1dc3f94b4140028025f76c808e7183102c1bd80829338b23d4444" },
                { "zh-CN", "ec8768e58985c6dbbd10bb5babe77930729efe8d4265a04d0781adbb00714b31fd1e6268a02f898579e0eda1972512c1bf9ad58ac9e08583086983483463903e" },
                { "zh-TW", "cf5f415c23964201f924c9ec3d662b618c3ab5b5a572d528ba07d13050bacb2f11e2c78d38fb20c57b54dfc8a65873a158080299134761851f265a70af5d984f" }
            };
        }


        /// <summary>
        /// Gets an enumerable collection of valid language codes.
        /// </summary>
        /// <returns>Returns an enumerable collection of valid language codes.</returns>
        public static IEnumerable<string> validLanguageCodes()
        {
            return knownChecksums32Bit().Keys;
        }


        /// <summary>
        /// Gets the currently known information about the software.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the known
        /// details about the software.</returns>
        public override AvailableSoftware knownInfo()
        {
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition [0-9]{2}\\.[0-9]([a-z][0-9])? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    Signature.None,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    Signature.None,
                    "-ms -ma")
                    );
        }


        /// <summary>
        /// Gets a list of IDs to identify the software.
        /// </summary>
        /// <returns>Returns a non-empty array of IDs, where at least one entry is unique to the software.</returns>
        public override string[] id()
        {
            return new string[] { "firefox-aurora", "firefox-aurora-" + languageCode.ToLower() };
        }


        /// <summary>
        /// Tries to find the newest version number of Firefox Developer Edition.
        /// </summary>
        /// <returns>Returns a string containing the newest version number on success.
        /// Returns null, if an error occurred.</returns>
        public string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent = null;
            using (var client = new WebClient())
            {
                try
                {
                    htmlContent = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                    return null;
                }
                client.Dispose();
            } // using

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            List<QuartetAurora> versions = new List<QuartetAurora>();
            Regex regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
            MatchCollection matches = regEx.Matches(htmlContent);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    versions.Add(new QuartetAurora(match.Groups[1].Value));
                }
            } // foreach
            versions.Sort();
            if (versions.Count > 0)
            {
                return versions[versions.Count - 1].full();
            }
            else
                return null;
        }


        /// <summary>
        /// Tries to get the checksums of the newer version.
        /// </summary>
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successfull.
        /// Returns null, if an error occurred.</returns>
        private string[] determineNewestChecksums(string newerVersion)
        {
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            /* Checksums are found in a file like
             * https://ftp.mozilla.org/pub/devedition/releases/60.0b9/SHA512SUMS
             * Common lines look like
             * "7d2caf5e18....2aa76f2  win64/en-GB/Firefox Setup 60.0b9.exe"
             */

            logger.Debug("Determining newest checksums of Firefox Developer Edition (" + languageCode + ")...");
            string sha512SumsContent = null;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                using (var client = new WebClient())
                {
                    try
                    {
                        sha512SumsContent = client.DownloadString(url);
                        if (newerVersion == currentVersion)
                        {
                            checksumsText = sha512SumsContent;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Warn("Exception occurred while checking for newer"
                            + " version of Firefox Developer Edition (" + languageCode + "): " + ex.Message);
                        return null;
                    }
                    client.Dispose();
                } // using
            } // else
            if (newerVersion == currentVersion)
            {
                if (cs64 == null || cs32 == null)
                {
                    fillChecksumDictionaries();
                }
                if (cs64 != null && cs32 != null && cs32.ContainsKey(languageCode) && cs64.ContainsKey(languageCode))
                {
                    return new string[2] { cs32[languageCode], cs64[languageCode] };
                }
            }
            var sums = new List<string>();
            foreach (var bits in new string[] { "32", "64" })
            {
                // look for line with the correct data
                Regex reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value.Substring(0, 128));
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    Regex reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    Regex reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value.Substring(136).Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value.Substring(0, 128));
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether or not the method searchForNewer() is implemented.
        /// </summary>
        /// <returns>Returns true, if searchForNewer() is implemented for that
        /// class. Returns false, if not. Calling searchForNewer() may throw an
        /// exception in the later case.</returns>
        public override bool implementsSearchForNewer()
        {
            return true;
        }


        /// <summary>
        /// Looks for newer versions of the software than the currently known version.
        /// </summary>
        /// <returns>Returns an AvailableSoftware instance with the information
        /// that was retrieved from the net.</returns>
        public override AvailableSoftware searchForNewer()
        {
            logger.Debug("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
            string newerVersion = determineNewestVersion();
            if (string.IsNullOrWhiteSpace(newerVersion))
                return null;
            // If versions match, we can return the current information.
            var currentInfo = knownInfo();
            if (newerVersion == currentInfo.newestVersion)
                // fallback to known information
                return currentInfo;
            string[] newerChecksums = determineNewestChecksums(newerVersion);
            if ((null == newerChecksums) || (newerChecksums.Length != 2)
                || string.IsNullOrWhiteSpace(newerChecksums[0])
                || string.IsNullOrWhiteSpace(newerChecksums[1]))
                // fallback to known information
                return null;
            // replace all stuff
            string oldVersion = currentInfo.newestVersion;
            currentInfo.newestVersion = newerVersion;
            currentInfo.install32Bit.downloadUrl = currentInfo.install32Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install32Bit.checksum = newerChecksums[0];
            currentInfo.install64Bit.downloadUrl = currentInfo.install64Bit.downloadUrl.Replace(oldVersion, newerVersion);
            currentInfo.install64Bit.checksum = newerChecksums[1];
            return currentInfo;
        }


        /// <summary>
        /// Lists names of processes that might block an update, e.g. because
        /// the application cannot be updated while it is running.
        /// </summary>
        /// <param name="detected">currently installed / detected software version</param>
        /// <returns>Returns a list of process names that block the upgrade.</returns>
        public override List<string> blockerProcesses(DetectedSoftware detected)
        {
            return new List<string>();
        }


        /// <summary>
        /// language code for the Firefox Developer Edition version
        /// </summary>
        private readonly string languageCode;


        /// <summary>
        /// checksum for the 32 bit installer
        /// </summary>
        private readonly string checksum32Bit;


        /// <summary>
        /// checksum for the 64 bit installer
        /// </summary>
        private readonly string checksum64Bit;


        /// <summary>
        /// static variable that contains the text from the checksums file
        /// </summary>
        private static string checksumsText = null;

        /// <summary>
        /// dictionary of known checksums for 32 bit versions (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs32 = null;

        /// <summary>
        /// dictionary of known checksums for 64 bit version (key: language code; value: checksum)
        /// </summary>
        private static SortedDictionary<string, string> cs64 = null;
    } // class
} // namespace
