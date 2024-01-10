﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024  Dirk Stolle

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
        /// publisher name for signed executables of Firefox ESR
        /// </summary>
        private const string publisherX509 = "CN=Mozilla Corporation, OU=Firefox Engineering Operations, O=Mozilla Corporation, L=Mountain View, S=California, C=US";


        /// <summary>
        /// expiration date of certificate
        /// </summary>
        private static readonly DateTime certificateExpiration = new(2024, 6, 19, 23, 59, 59, DateTimeKind.Utc);


        /// <summary>
        /// the currently known newest version
        /// </summary>
        private const string currentVersion = "122.0b8";

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
                throw new ArgumentNullException(nameof(langCode), "The language code must not be null, empty or whitespace!");
            }
            languageCode = langCode.Trim();
            var validCodes = validLanguageCodes();
            if (!validCodes.Contains(languageCode))
            {
                logger.Error("The string '" + langCode + "' does not represent a valid language code!");
                throw new ArgumentOutOfRangeException(nameof(langCode), "The string '" + langCode + "' does not represent a valid language code!");
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
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "eaa4e4a885df116ea6e2d75a786a0b99776f30096bfb6bebef53ceb0f163b1021151620ed0d6c6d317671cedfeb3a0f59d9683d7df9e958dc598063f9c99dffd" },
                { "af", "3a297dfd4e4f70b4c2044bfe1a7f4a91f29a739c0e8fd1c19f90ccffe4f40ec2aa05432363cbe35e926a1303d3e3b08d2e1d08a0a7e11329a5d7fa2606602ef4" },
                { "an", "df4ab35d388c091d0f5c63ac67d28ce331cc2f6cfbae0f4f26ba720b03a8f784f4ce6cb3d245014b58177e1908694a1a42b7599afab0721e1f982f94f6710e4e" },
                { "ar", "356a16206300f52d8b929ec17f2246b8fc7349a26bd867c67e04145949e8cb21877435fc8f8f6e80ee999a0eead6b0cbbb8396967862dffba90776c6cd060bda" },
                { "ast", "0ca652a42cc1b33fa32e5bd590b252babbe03070363db85a039ff9aa92578787d0eb7be1ca3a1653e0109f6866ccd89d890ff91fd2069913c991e491fbfa5de5" },
                { "az", "30896d4d007b9c124f4e152273f3eb16161d259c2c0a8c9b32605737b82a0378a832140aa855dfa6de6c4e6b9c6d4b3bea809ec49a0d7143eab9e0abb816a412" },
                { "be", "5c2f62497d1783346304454670788902f44578bea262d57f7f23166feb7a6d7854e34cbf5cf36fcd23d06700b0a4de4b56827e9446cf95dfd40009205b78aa39" },
                { "bg", "c589c56db8bca7921a36d7981f1255484df84b1cf46675d3b199821827ec2c8fa0efe0442257f19fd21a05ffb287443c3b640793197ffe703ad743ef8e368cdc" },
                { "bn", "be9ab9f3b50cf98d71e2d342077c77bb3b15381ab38f1d1d2b3e6b1cac0097afbef37de5ea41c2b8364133e4b63aa6dc3c365052443d8488e5817c162e0449e4" },
                { "br", "08abfdeb4a6d5d67d562f8e3d039e739d2d63e6bad236cf5a05ba944c319c462b29625bd45f453aa7b817ed8131c194fe185cbf13e2114134b874a3b4fd4b304" },
                { "bs", "da617f962383bc97b7c11853e87cae02f5d6ad687995e74c61738cf637560642719c499de5703af0ad08ab53ed9ee8aa793b081bc2a65632239a6960cd576f83" },
                { "ca", "c985557c013894aaa8c8801a59a0618bb4317d04cbcd10138e521c2078ef3812999708a88a6ed303d1390f97198af00c4a8ee462be6b86c02fac41f69016958a" },
                { "cak", "9a13ba6f95abe52bfb228589180c7a23d16bdbaf660233715986fc1dbe2c3ade696c86f92a911feb1e5b26b6c68475248c6b11a8c93f3516d3b0e0fc7f4e2570" },
                { "cs", "e45e4f1a005afdfa4d61553fffd3a7a3bff53991d0c5c9dbda3825198b4ca2e4ab0dfb62afaf0b56a8c2fbdac7c03ac9dea3078d37d586b2ebb99a88a50df256" },
                { "cy", "fb2b68b8fbc6b03502607220d8c47b267d25f9f16e1b2e87a02f145ff27b4b118cb977854216acfd8b9a886e103b0738d57f8e6978ad16bf3eae29b61326ff10" },
                { "da", "2ff2eacb045b044b220bd03be80c051c381b44d7025fd5b97efdef22cd436af27332bea08e0c27cb291a0452ac57b5875e88b35e6e70a63cd2ebdc48fed74ccf" },
                { "de", "8dd12e442611bbc43a3198afef32c4887e28e0fde55035fec544a419b720616e5c0b4b3b260a909b0eb7a4f51e0151c2e6bcf573066ba7ba9c9aee882a8f7084" },
                { "dsb", "f57c5e8cef9dcf09978b9828d2a38807edec1905300ca35ba4e4239b9972da7c5eeb7fb9be9411174a459eb6f5dd4ba045d1cc454218ecbe761a686232fd4b56" },
                { "el", "ada6e51f5acf7850a6cddf6f9b25c6b3d327e5f1773ad3182b2f40165424e5a963d6f16e6564c2c3410de076d41e30aaad7989449aaea150073647ff9f3942ad" },
                { "en-CA", "71cfe33b1557c85b95ad883b1aa4260d9e3347f9e5b7ace38cc2d5fd5ce3b9d4189b8d6b41994e37f6d4fd3d9bf8d6f4f8573e42ad0bddb422bbec1e7e86b143" },
                { "en-GB", "a749922f0d296725fc220ae4d7aea6a83a5276af1826400b2480e6ea7405678d8a4370299bf322c4bd7b5cbb92b7e97d0bd49a1e6bde3a8658b25e7b75e6903e" },
                { "en-US", "873ce8a509a92ad3e077c5197b8126b41eee35f19fed674bd4e881c4a0e96dc2449f3fd4a62c5c51fd34d5df829304f127684e347011c0b6a2b0208251c86017" },
                { "eo", "01a9b080f743026a684fe2eb54395f30cd1ae912e3d24388a975ea257a0a41bf1925af54caaa1d770f54c22400486d0322860e91bc43094dc18a0565db539648" },
                { "es-AR", "3f841bff178a5e5219a97366815783db8fcbe52f27afab2e92d3c577903064aa267d10b77ea00178a9b735397d86cad9fac23017df6964fe15bce19ab1be2de4" },
                { "es-CL", "5c46a85da1cce0947a722eeb4b2482ec4d852d6a3e53a2c3faa47cff6ace52bcfbd6a07c7769dae1973c4f8012ea0d258dc8588fdb4381e86d935ca0f373acd3" },
                { "es-ES", "5c5b439f4da2a0d8a2f39655e8530a257c3e3370c5d98ba00bb4498855bc3d0e914d189f0b16b2b253c1951ca979b909205f932ade66c9da5a69fb1d620cbc54" },
                { "es-MX", "b5bd3203cd0ceab11d9cf81936cf613511361c8e2ff5ce531e39c7c0e1ff2305d10bb838e995bbdf46c9ed3ff43fd2c16cca8de1174d7155a3ddf660a2bcbf2b" },
                { "et", "5366c918e113143507e5bee6d6b2b4842061eb7cca2a483435cb61579d29adfc941aecaa924528fa3782f90c9ce81b17667ef9cf69d75fb8df1ca5d3c6c33573" },
                { "eu", "5dd7b8422d46316a2fce9e3ba591f2016369aac2cb03a1d62894443e8d3a805d4443627963a0525f06ad2de9b40963c3d93fdc9643089d670c9bc10e756077a6" },
                { "fa", "2fda6cca0ca5765b717e9894cb2af59306be3d6b0128d425890b889c418ed14c3c66995ea59f54f978abe99f426a22229d6c11e4f5e67e1d970af3cca48e6f7e" },
                { "ff", "71704cd0805b0db882e429f67bd1ceeecd001cbbf8cbdc7d0a8a9e5af475fb4846a6e65e1748292cf669cf87295ec23f650c558ebfd876f190e467234ff2610a" },
                { "fi", "79ae323b2451cc9087c50fc1e4c1c706a4ea62f304cc00b31a6a82654fda3febe3edbfda258d9a9ea78b78bde5c07e828859cbf77826473e4a9cc26d4412fa4d" },
                { "fr", "33abab7020ccc99d792a63ee145d991c86411b1149b978935c2788f590436a26647a917efcd0684c1290641ae0de1228287f4041223f8a9091d443a0b3ef6978" },
                { "fur", "b3447f044577c0b5e9d839727b6bfcddb539533fe46dba923829bdf9532db77c3920607409c7167f88d04723dc1af68b5ef3e51ec0902bb65c3c013e1ffdf8b7" },
                { "fy-NL", "c0529599bf72d45f9ab195c560b02053a1de717676817c2c1a6e267f4639889154f32da5bb87ac7b48dae4b973f317734bfbd9ae592590cfe70517d7987a3e92" },
                { "ga-IE", "f7b6d164dd198f6bc8a077158a65d1efd2202c1979608c269759d370d60b55a2e8f96e4880f1f9cbd171a2c2090b64926cbb68c6777050ce9efe172115b49c82" },
                { "gd", "95ae174ba3fe924cde4236e11384ba7b50354112c404ebdff5d2dd337162f0262198064da7ea016cf85f81411cbbe1a256c5d790e6fdc8d01ef958df18b67e04" },
                { "gl", "ef014107b3a9c48bcd0877b4049bf1760404d740ddbd72b669717b69217307752ca972724ecbf4babbdc795714c5b84e9e75b46553cf7aa0d0158612dc901338" },
                { "gn", "8f59a513bc3b0fd435934e4ee357a8b40e8d8eff3f9652bcc2c9f124014ecb756e0075fe550a93a312e1952504e8e410f898a404f203457f5b907b75ff0fa58a" },
                { "gu-IN", "7b1482bb87b2a7c455d46da6e65b3d926c6d36e9dd409e4c0df19a63b0a02bd559bfc1439ac67374690dbf2ba7c584cca9dac576fbe68754fabc6e83e904e31a" },
                { "he", "3577c1eff9d7a2554cc34c42554d10e48e26aec3741c053df4cf922097439617dfcb4076b67f66b5c0b6e852984eb011a93d7c5e22401f62e22e27558a81d8c6" },
                { "hi-IN", "f42c54d4370c2ed2c75e83a1608dbc528e8293c0237d92528720f0c90d1662e6e1d7c9692d9e156da97205fbadf1fbba5d5437b6e701480fe86fd603b57f77a5" },
                { "hr", "510d9c814e793caddfca214a0a9a59c9196e26f2e8e2705acc2e644f4693369109902c6df7a2d3939c102c35b08ee80b508c9c377335bc9a2e27f50e138fdb33" },
                { "hsb", "9f40347a86774d9fe16e8ab334b0a0cedd3019dc78a7fd0ee4d7942ce813b567022c2f1cf4ccf2825e873271e29218bbb6237576d7b750da37a482207d105532" },
                { "hu", "35126c1f3df0aca80a551ac42e9ed4be7cbf87cad81cd2dda50d961c126535becc1c172835891ef8590166424c8a718549e5e347fe98e00f2c29df07f9ef27e0" },
                { "hy-AM", "fcc1c78b30f0de85fb089af8c23ba883b23a442667170017865c29154f91dab23e4c37d2dd02b4ab3bd9c48a1d1e63d9e92a5f4392c943dce04b97eeddcbfd76" },
                { "ia", "606bc0f8adb284d560e0a2891c9ef8d3525fcc4db07a3208bdab6849c8c43eb035b4e9c3956e12e6de9174a5603c21774a9b2b16d60644b9d0e57a5e46839a5c" },
                { "id", "52568f81b20f9d00fc831aeb976e6318ab641fe41f8be650542cb83ee022244fecd35ff6956910a063b5843f74a123c2dcf0ecf1dc4eee313b8c26b387d8b8ae" },
                { "is", "016e75b75a566bedd103b5f13230432bf852e45d484774b89783813251dac4dbd4754d17919a2fb608897abc798dc7af94e5402ee5038e093f7f3b489c74ca4d" },
                { "it", "80fb9f525176da2ee8e1d4e479fd7fb6066446749530693b6b70d724558e39e2d77beeba3beba3af6e93472928958b492edb572941f6b30c1b896c54eedab762" },
                { "ja", "57d230a0e992a1eebff7e83ff2cdeaac0cf994f253db76456e5fe844d1d08cd460fecacd0cb8d32cbd485d1210fb538c2089d36c4f6fbd353b6c8bdfe26459a4" },
                { "ka", "2afb2bb3d1a6807553a19684583a4fba708d22bf022d821230406859bfe87295ad9251422ed2f37437de9cc1fe62f5a52c830e4dc614a37f956d62a03ba07445" },
                { "kab", "2fe39a691f7ade39eebe8ccc280d470cc5d32649fc40df4028c5fc9e721bdae58ac7ed394dc36d4e6c85abe875ad5dd07739713ce1244a8f070e8af0ed342908" },
                { "kk", "13d48138a4c7512ce04561f4cb82c9c64ff86cffd5a1f8005e7734946f8ae6d613999271ce7c512d302b169db0c33078b73c42ab5f77347c74fb4fe1a218b302" },
                { "km", "282421875bdc0c024cb50fbbbb19961bb0e01bcab0742566f4119cc121a96a86c3d7981f80ccbeff6b1ddbb2560eb72101441213adf8f6cb95e057ad18c9320f" },
                { "kn", "4a20c91c0b29fe3436bc0dd95b96e29a960637b586c8db1c21d66281d6f37cb29950cafcd0993e9f3f3e828364d02d4c9190f26ab5ac9b12435936ac6308ebfa" },
                { "ko", "f14aece135255e9b8bb50d28728a019a5a8ad2eb1ba9156e3785f465f161e23d9e559fd4c5b61a446bbe2c64f355acd9b3068d8026448b9fc9925ddf455261eb" },
                { "lij", "b52e4942f256bb4ed84acfdfe0fbdd73b4f0d8822c137dc96d19b3ffe0f0ffcaf2990300f206ba667910ec4d9f77d2fdd44bcf711c97e3afb457d87b7ea92e89" },
                { "lt", "d7254a78fcd7b794fca4cbdfc558d3a3bb9b1beae9284edd4b074175c18afa91e4f5323b67aaf799e3da4daf71bb9b645f21114000c18460cae45ad86ca075af" },
                { "lv", "93b01f42e45b731b81f9c14cae7c2173b41ff1c1c23ebca6e0102dbc4060e4e67c9e59921fa3170a636e66e3cc4cdcaa11cbbd5d0e7ad745e139eaa9fdae199a" },
                { "mk", "88dc71300994d845816f4c5520d5c3acd0e1d23f37d4060ad283608070ebc82ceee4baff6e58a54f955b96d4c2e6fa7a43151f9e6809f5ad3e59cbcb071a78f0" },
                { "mr", "ec9fabb0cfcd7bb518779324c9afb452ce076992f36d1bdccccab2c123ff60cb9396dd56cb430e23b70cb1f016f2bc2bbc86636ec55ec9db506ccde88050c0fd" },
                { "ms", "e97f74e279bd396ddb857a859fa553477946bf9ae94cbfcd99f0667dceae95822623b46088a0b42ead9e70ca5f6a4f3a9e4eba16d8a76c5003410aa9dd646d47" },
                { "my", "6aa46d3249cbbf501874d75dfac95a5443b6c2f3ab7236c188e2330f400d78cd9e325c1f0034d37dbf848a1299410e39f28b4c894688f648b3e5ee5478702960" },
                { "nb-NO", "3dff126911dea3003c4180bdb102fa43438f40bc159f24aa848db29150783ba8efb5ce8c92089984a114cec41a31b2ccd84ffc99863e42a35f747f1684fcff6a" },
                { "ne-NP", "79517132797e76863393d1dee51c37030f33e162a23fc8b6c91decc7f8c45265b238d3750002b490bcbded319e15c440033600210eec4ef0afa54ec423240fac" },
                { "nl", "c167ec3602377e711591f70d19ac49761715b682ea45ebe097a5c7c245b15d7924e7c936e4dc8c19f64c80ca7a8d0e0f65f4a6e37fe7588eb369136681d9ec0e" },
                { "nn-NO", "9f02f13387bd6d74a5ab159335829c881802158a393d44434bf947dcb61c0e9dc0ed1df34bedc65946c3d5405d43f95cc9c6ad11e257d10a8e83bd2ad972a165" },
                { "oc", "83ed4839e62df627fddebf16bd577ba7286b3b89952fc3514d40883ae5fb34a8dca3a858b9746953ab69e217fe7025cbe2b7a0488c06b4294df42dafc01b2930" },
                { "pa-IN", "c7cf39181465e03010fb04586ceb6ff5ad4a9988c1ef82414da37016a860d9d30b25152f2e21277cc9db9a56a9a6eb06b0bf840133297b4932c6d6eec3090073" },
                { "pl", "3ac41230b1e54e8e5dd9309670b2cd72f7c023e551b2b0cc165ed53f2cc4f5195b868339eb4d29a6424db9cd26a09c9227e3adcac09a553672ea8a6c0cec845b" },
                { "pt-BR", "df9388aecdf983ac2b42d3723777eeb26cc97707f3ec77e25e4fdc6d8c4f5acd23358ae3646a2d58f0cbf96b18832ce6052382ba722a94847f3d3900e2dccbce" },
                { "pt-PT", "616592a090600d62ee813563ce5166a8a6b5bff76e7a7f9b634bb660b1d3712044b99173362721d60913620e87f066ed657cb3ea7419ed9730146cb4c719d066" },
                { "rm", "93a38c4a663a4b9cda8345a7fbbd9171f65ef95804916005d0bb312272a31ac176fc866a9021314fd897df06bb2e22019566bf38573b6cb7bfb89ea6f3e57109" },
                { "ro", "f330cb07ffb676b92fb5dc5b490bf790bf47ecb5d3b56edb713f7ae6b1a47d5b7b388d20b1d56a6bd8b5967b62f1ccfe4ce71a6e98a084dc7de96cc7bf21d70f" },
                { "ru", "daddacf31ef1039eeccd9cb64af5320496cf49131c2c43978b07bacd4a698e80b083258dd7a72c382bfeb447a40b25b1188a2587a0660569d2d7247935df51bf" },
                { "sat", "922c18f68b012281fd23347fcc4ec205d8c0414c042069e2bc21eda6be28fd5e16ab24d892ba7f8efb01eb9303c49a3a2aa4bcc4643c3958a26e39bc87deccc0" },
                { "sc", "8f5a28e08174294730ff1e23b19286c6f695180b021ccf2e6a590ea49e9e855249181d1ec6e3b1bbaac837939b431db47972a623255f683bb73f74411935ed3c" },
                { "sco", "1bf0e6f4775b1f9265f009f3c13a6f90845579bb565c5e8a1663a84d73136ef70a8da91193aca6100f7bb66dfa8082dd12365f7e9a5e1423dac6a748c68d353e" },
                { "si", "ba1244fb682c775a5cef5746ac3bca7281077c6e9f376db22b6543884e7e619b970fb6bb01c93e079d3353468fa47942351aedf7be8a90d061c878b526452fab" },
                { "sk", "d5c6243436e132b96c2081c371a511d47bcfbc5c738f48b2369a921ea36559aba326a37fd6287ee66e57970d259386ae9ef0d2f97d6964326d41ee2489b2c3dd" },
                { "sl", "6a872e180668e3eac0ea5b8d4c1ad4a1a51d97505ff2cda07a54583623d11e56026f4c87c188ceec956e4cf312812ba68b8c2ea72d381da2251f864c90724c3a" },
                { "son", "137dd073900f5457c86db6268e46039ac0bb65d63fdaa3d5b9d8a98bdbcd878af0eb9a40b79b01cc417eeebee1628ac0044888624f834c7c7feda4f3079dc0ce" },
                { "sq", "e016c02fb1f60b47f7813bd03473ec5f8853f5c23750b1e0f63b50a88fcb6c15b8eeab3e23ac72e812422292bd2ecf919aa509982756f79aa3a0754d5827a4f8" },
                { "sr", "6e6abed7ca4a4dc78fbec5dbdbd3d429fbbc73df9c267de987a97c6c0eaa8a565bdf4e73b945deb58d4c9f3a5cd7c0a5dd81000c77f058d467b9953e45f86d14" },
                { "sv-SE", "983090e034b782c82adb0a599a5b6db03dc949dc068152c77161c04c5b1afe3b741969dfb69694ec774b50c7c008895c614ccae2ae3cd2aef7f9f709fb9b3691" },
                { "szl", "7b814a39826d85fdf5deecdcf16379f7c3b49afcd77c031814dba443d89564ead512843e53eff891435c13680b25436619b09e7e4f3e2cb51bc421826fdc16c2" },
                { "ta", "1c8085f5a9a7436d362715068a1fd0d6aa7a1432260989c02b1619c4d379fbc8fe4528405510b170b4d141c1d26cf33ef4aa75b382512fb004fd5a8e83ca24a2" },
                { "te", "e1283e12ce1fce8b815adb7a5708c1d4a919ba9ae27fe4c21f887d459640f6eec9d034e6bc5fd9150c7aef7cf2dc8ff562eb5a0c5c2371189517117548b3f9ed" },
                { "tg", "2e53a91e53195d0e4f0598aa81fa78928e191d6342a5596ce6ad96321a0b9272c9ebfd09e4e0f0ea998a8aabd177640f91e3986fe8d10395bb6bd6d1ed98a17c" },
                { "th", "75d2ab88f7dc1b83084903225fb03ad6298938fb19c48e716668c20106072af8537abf49f71b11cc0f37aa9f9b8a12a16b9fe20567fc742498a1d3afdfd83789" },
                { "tl", "826d6e012d98d07d605c5b4ffe12308aa40e14ad167bd5d5dac2a0a41684d61538f0586ebd4a07e1bb51d0e249cb1dcd704c6d4a1d4979a5520a22b41bc76f63" },
                { "tr", "b33c92b0ad29b55a1ff96bed4d5f165f673752d79a41c3f39a341c4095f216a4361f4f29cf56fe8e285edc057725f295a9e6ca62a2d8754893d912af10386405" },
                { "trs", "03e88c2f5b1be4b5f5e0209a9c0a1dbb6f5c150405fb09d1996c2cda59e3680da25fd7da548d8e0667a77d5ef4425647d88100cf810d0d24f779fbcaa77188e1" },
                { "uk", "100b35121ca0ccbb06ba5d92043b7f8967264b3b5ae5235aada986ec12873679140782973e7527d9f1d87c124ceae9ad46a504a750870ccb7f94795873c12194" },
                { "ur", "48bdf09eb8c42a3cd831112d9c00476cff210dfc6140318468c6e3dc37c21dac9c049eb43bfd57c38298254ecc1e17d043a70a885afd7483ae523ec4e73d5b31" },
                { "uz", "51f048a90d96505dbae2ade093e816995f5b8c8a385af54b021e212e9a5a1e3ffd1912734fecf10729f055d40b0c626a5d9d5deace5f8d8dd6a4d81423bfd1b4" },
                { "vi", "2a7de4fcb5a068a266217cff6bcc59e3963666e2ad622041ac39a898766226c6f4a6dbfe0c486a08faa717b21490e8b0ec06f7cabd9507fab16db646fc1874dc" },
                { "xh", "7f1d860c8b7c49b8f865c78de674f5395938be46226a6b960909451389685fa833e0e173b0c05b1572e965e461f97e90f8dc83d10df2efe6e63bd8108c731207" },
                { "zh-CN", "f6765623c3f11c2a2f973337219ddda739744842e1a84675aa17ea1a2a9350cd88318cfa44621e3c68ee5851d2a6457142686f82209053d9b9d449320044e0c4" },
                { "zh-TW", "c2af4b0951aa994a8c34702219b42817e28d7ee0d6925c56817e0d63111492ed8af375a32e8df1f80795a2d4c56b9068188910c916790685cf201286b9569e4a" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/122.0b8/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "daa960aed132367b85239cbf2e17fb51510a872eb0fb4da7789c17353102eb32627cce010a19bfeb8bdf0eeb65252b5e2e1c6139d24b2869514ff1b776001c81" },
                { "af", "0af703ff5db34911dc6cbe6e3751d2aa9c81469dfb04c5ada5d8bfb0f91e2a44200ba396d504b71f39973889630012bf0bd0a182634bf3b279477a8da558d048" },
                { "an", "df5bf2c9d20cdf873afb3a22165832df2a65017a6af0532c1a0ced23d517385ee0858482ddaf7f14dcc457620e278b3005b354e474f5323576cce24e628c8fa5" },
                { "ar", "be9d4dafaa85aae70dba25b0fcf0a22310346a2b1a74b21381ce33b8afc2b1d632fa37ef1e99a86430d6289a2c5a554cfba56d4a13d67fe90631286856b1976f" },
                { "ast", "51c77db93bbb7656969628e839e97fbaa0279daea9ee2fb76e932a6617698e76460b0f18c7fa7497476178632d167c759a6d58381a704bdf8f5468778cc496b3" },
                { "az", "535045bf33ee36f7617507a6cc338f0bacacfdd04ebb234843723f88e67bd2dc619db27be37b1acc70d55aec0ae2291b56e0292a1f11449bee4e95f41c86012a" },
                { "be", "ae3bdb8d30274b405ffaff35a2ebf576a3e95a7e6379a910f3301aedcb480fb386bc1098e73d9690456a8e4605b2a2c101bfcfb60582dc11a035472d33f99426" },
                { "bg", "adfc0082f0dcf77310dbe3bf72272ca76e55e000d3ffb26e9298393d02be86cbbd821727b0e08af0701e00d956d1dd55fd88a1353e1ed9328b26f92c33c02052" },
                { "bn", "fe6adec7f3d5af98127bef7d97f26da7e2fe6dd4169f8b492a24efb3759f1f827883b830815968aa60f5501f872d7284edd2edf66b349055dc879fd0ec654280" },
                { "br", "1a0f2232a0ad9deb42fb3579efa2ce1a00e6fb6f5e56260b04ee4a66c9536de915cf649ee4bf0caf4f54c56eae41d72abd344764e35fefde4f1c9959d1105a2a" },
                { "bs", "fd4486d44e8cf9cd58801aba4110a3adc69c51692bac93230a52c02064e7a7da0cc9b2dd9d2f415713bbf54a076adc9c9ec1581a7a490cb5437dec9c6708ec95" },
                { "ca", "856bd86d643ea4d8c08ce980e8ed8165c28b14e3160b8f916898976690729174778cce81b757ebdda382e1110ecf49d7fbdee28cf51991229bcd734b7956b6e0" },
                { "cak", "2ed5d4ac1e5b636b1a13e1ce3e17b680ca03f71f6e36bc11fbb2e6af93e867b137bfb091af619227f7871a062cdbccc8b22d6f82dad54d224917aa2d72687bb9" },
                { "cs", "380f235daa1a3395f4ac8476ca5de312c43daaafca4219d7706b51a02be2b0d00b8cb84cc5fd51ac7bd6c69a912c3b77d0a0e9a4e13861249177258558855a9e" },
                { "cy", "2632a41d11487fd620c588c244e6b8130c0a843609c06c183cc81d2738ddb25c57074ba21fdc6051f48c3865fde6db04a89f8d17b40059c5b36aea311950ff93" },
                { "da", "7fc39c3a310901e6454aa851d860a2dc406186fe852432cad9c97da3c153e934e2f57b719cd25f11a2887368b284815b5ce827020f316072c4b3f5edef691b7e" },
                { "de", "172db143c47e9e350643f8509b733399c9a475c44342af6b61dd6c63142742f877f175074d8f9ced0caf6b418758d9b6df7e3cbe3f3b284d3bdb85e7fe1827f3" },
                { "dsb", "4ca4aa0edbe7f19d2baaae91be142cd9954836ac6edc4eb2546dc4837559f80300bc68ef1afdb4a40081aa5c7f26b5caa4cf578a9dbffed244ed95dcb76111d4" },
                { "el", "35d821a1637d4ece9bbba29ac48003867c08f75618f6521f10ef0f08b1849c0627ab98de048037d34ac17c9643eea4df3fa4dd73a4a4baee0ce821b97828f03b" },
                { "en-CA", "d81ab32b43220ebc550f53c136efe94caddec1f97bed856c60c7b01a4f440a349fa73ae43478afdfb2833876a620c35eaf0caafb27cb5d2b633ff2780594a798" },
                { "en-GB", "95a194ddc57020be898055c36771cd71fc587463dcc34c4a6a63044faf58b250df8abc52a9de27e748a67605d040c3fe58a46aa57c98470ab42e72f276c5af7a" },
                { "en-US", "2267607a98c85bf5aeea31fae00f23ed027767611fe3ffdf11bfa1d50bfa583aca4f48378889f9ea5816575946416e6a36f6c4fb6da296f33d8a438de665673b" },
                { "eo", "8ce9dca5db20dcc83aa3ecdaa31a453b03d0cfe3454cdfb638512a3d6143f709309b0e16195cf19001923fc2d808e8123e70b565a9cd91535a2cf5636d33a95b" },
                { "es-AR", "d502503a9a46be4e940d98b1779ccc6403f2e03629b1758083808ce81f05a6d31341fc8fc59add8501818e183e14900226785279a65e3362a30e539a6e291e5e" },
                { "es-CL", "eac20d0cbd60912a9787a0bf49cbf71253b19bde152e3a009601944dc985c3ae46044c6b132c2e9a46a21c33b61c72ecc401a887eb0f0e4656d07d9f95151f5c" },
                { "es-ES", "26185584ed7ff87e17e75849b13ec02be873f42906ad068baf26471779f84d1c3502e2e04409fee99e5c2d26661a9db97e68688be0eca6ee0b917974beef23bb" },
                { "es-MX", "5fd005d8788c1ce8fdb0315f90b489f4b9e6ace9138e6683351f1a87c90c6b0c1088902448ab77b36fac3c1861eda364faa247c47eb98b0db655cc1d964ee50e" },
                { "et", "2f97945089a5ecdd836bdaebacee3fa7c1e88320d65b5a3becc9306e7b52a2bc3db0ec50bf8e057e3eba8c73c032538ab387fc38ba6c8009509180b684c618c3" },
                { "eu", "e0945cec83afaf1b56e06fe8e7bc6a4e60272b89dbb89a373e9b5e22104ed3171fae3004c9a58f22333adadbe41c8ac5bb0a1fc2c1b3d2f88a4b5f345735fbc2" },
                { "fa", "afdf265c0a8a34384717bbaa45b9f914a7305f36e7c978b68736fefc22403ee6f28ea78c63596929ceeb423987e0561a2da4ee63688db9772d2a3e19673c21a8" },
                { "ff", "a585ebbfa84e33fd292897aa6f27822f39c3e4b802787dca4fdd9bfc8b704a5ca9cf1795467db708675e1aa5e5bf6b386eaf1725b1fa75f57edd0118dfea435f" },
                { "fi", "dd1d50066a5aad6d90b4e16ae1d2360788fab48628d06a343a3b49041ec840cd56dbf1d78e821bd22f63b01cd342b29936a56da2b5a1101fc5ab3457f149a3e9" },
                { "fr", "f2e83d5c077e34cf10b469f74789a4958955278cc875a20cd17787dca699af137a968fb1752eb03a47d555132dd381a911b0f16295c668f7d8f04cb386bd89e1" },
                { "fur", "b81ca16454df8c00acfb6d9e560f4694f14cdf658c1f74d8596a78d466cd04248fe703f1b3e78d2a0a788b07088d1c9a852e6997eb568a96f0d9bbaf2c2ec1bf" },
                { "fy-NL", "6d2c265f06112854c6ea2450b7ef5f1d878ad738617cd4b708189416e155e53bdbe46a47d36be03c5f90359bf7912d08e774a53343f32aae7e182b3aa0bc35d4" },
                { "ga-IE", "5deebe0e9da3915ed012b5fd117582d80ed4f85ac43ec835d69fbf3c225fd96c9fe07efc88fd798f8c8f92653145cb83c94b7e0ec6db7359244133efb4b96286" },
                { "gd", "c5661c5b3d0333e2bff66c038cbb15818329e66677c8d85c18f31289440fad386619b1f2095a02de900f00e927749e9039ea012122e150d3290ce98e9abc8de0" },
                { "gl", "fdc10a5603167d3b87a943e0774143f83ed99e9d8e334f1d2bfdb8c330275dc0edee824cfb61202cdf3148ec9976175458a56d41c2cb0bfd065a9768b1a0f2e8" },
                { "gn", "6aefddc8ea4910e1e56a7609a45cc4153e5d066f3e28e80bab71a7afe904afdb0529691badbf121b6ee3ec0e891e80814e555bf9e6c2b496a7398ba98d0ca4ed" },
                { "gu-IN", "70b3dd7e2f1233da711eea25a9f52684872c561409aa883a1ed3c45b3149664b17e09920b217750bafc12ef2726f1c8721894b79f252e015a1c3157f8421a220" },
                { "he", "ec8999cf784d0df591e5746d7b426b6fa0d6ad31fd17000b939bbfa5325ba7dadb5dabe18db4bd9e8beb4198f3ce9d92c491342d8ed1c42e732335f3f5866d28" },
                { "hi-IN", "be7e8f8b51b963295c771db0654b9b80570495e755261c4ca387891550de5f2be00d314ec54886c791e63e6eec08e3d8aa1700eff5d4d0f9f8430650af69d5da" },
                { "hr", "4f6c6f1bcb8cd2716e5daca79a3118fb975534355b265db2e7f22eed7cae7ae3002d8eb55b66fe3a0234580c1e6ef409ebfd6fdcdf8c80361215eed2af52adc2" },
                { "hsb", "279a9a096349e050582df4704a14f027068f2d607eacb686aaabe654b75fc20d2fae8c12b43749e13cd17281b4f78a70f52663f25fbdfdcc5e588a88d86062d6" },
                { "hu", "f931d1f102945e40eeafdbf49674f3b1fe745740836460b536d91b52e27cbb7669158b6fd41171cdb641d84434c60327c6f82cdc25816fdcb1d9dd1bc2ddec56" },
                { "hy-AM", "5117b4d68bb0eb2ed8dcd531f1227a5399718b24c3859800add57712c1ccb0cb61164282eb5324c95c1cf64c95de87c0b69d39b03c288c57fefb7630d6747e61" },
                { "ia", "349a23db2390de1e90e195f1338cf899b5d8f27abd3a62e17128d5b107a09fa54a8e9d99b32e282280521823698f2263943c35bfbf852cab8cc42b953c427fed" },
                { "id", "82822a6531e1c8b20213f6ddbd03cd24500cdce268fbc4e8ba0d42328c5b695eac612d0671999b358640a3ac897c3809b13f4c1710f7f9322ec1cc3ed862fa79" },
                { "is", "06f8ac9f3eeafea71793c93c5f75fcf51824c4cb76968ee9f3c51037fac9a6b9601ddefe92f5f264c5d3a3b70f2d698f97bceb5a5ea4e155e83c465d72561be1" },
                { "it", "e5a5ff56a076c88ae6080307d172113958f249bf91c824a14d2b1e0cb7a5e893b0f2ab34076ddf3924b4186c887e2a4e0c5a7d0ffa1d778e07c5d700549a277d" },
                { "ja", "a74fd63ddd8637e1a1c2eab3ca2c97984413e439ac0a0efb9afb7d45bb81b53baa7b2fbeb9cbe59e97fea6fa6cd0451952614757f3c346dac16099e8c4430569" },
                { "ka", "023eabd653d95efd8d9cf915c88782fe5ad2b61be0a77e2d5ac3546d9fb347095875885a2b3f123ae13d973f39db2904f0fa91d316447f363ee8c18ae3c3bd9b" },
                { "kab", "819f7c5267b629487d0103234168ad59df78cc2c5b652a280ea64630565bcf60038675706a650ebe7208d87ee51dedcd3857621f8fd6044f43238d79bbf97f10" },
                { "kk", "95216cc995794067ee1e17622152d38ba744035fb896104d7fb210e83453f200e9c135a1a162f7dffb98ccf1598b70f4241ec884c289cefaa138e262acaf9934" },
                { "km", "5758b9c4778670d45e5dfcd04447b2adf178f37d12aa844b3e36ddb849170d9b7c7c5a0b16cbcff580ba0d64b2d063e59e4de99796558f325f58601e1e75ed08" },
                { "kn", "8fd1c853f0ed321e35068a01cef3dd5695f834b5330cb8452855d7e558ad75455e8c672c46c5aeb23e395474c8c6955fa8e99b99307b400196af600350ebfe1b" },
                { "ko", "38cd63a58c631d681ff2bcbcb2d20281f2984f42a4b9c1e3ca7c5f4fa247728ee909cf72e96ac664cbe84f0c5a3a038d463021d2f3b9259df64ccfb8386aff2f" },
                { "lij", "1e58d1412ff3d176f91ce3d829abd335c9fde49a6765eb16f343c2ddf82bd0e6470dde32ce814ba880122a8116185de36755c7233c2e60924ef366667e4cef8d" },
                { "lt", "296485a02507f6ba694c01a81e01b6280f6341eb2f263ca387a441576d8d31f6fad77150adc3e22594f1f43a4a5a8308b0cb78df4dbb05441448cdedacaaf15b" },
                { "lv", "2449e8ad5ddcc6a1f2eaff4b96fd262ee56c37eb272955b45148a5cbccff1e74004bebaa5f09c488ae425fdac37a7ff48ee20a1c60d6a82e9633f0b4ccbfcdb8" },
                { "mk", "36ebc5811c5a1249668f0675d6b7c38f99a8294a0fe04d86176d522f42592b4183fbffd8e3c6d0ba2b46f5c8593380ac1fe95c661c8a4618e39d5cd3be4d4834" },
                { "mr", "55ad36ecb552c7b64a20423866d396c1150cfb7254005df6ca345c18bfcdd054173b8e21606d6639c1648b3db077ffd7cf770440489ccdddb876075663c18bb3" },
                { "ms", "8dc96dcd798b51a0269d6e8eeae3159093c9772dc500740b12e64bce0d27c6f925cbf99ec98790f4da590488565b3c9e8d09d1f6c3f46fd5476a42036fb351f9" },
                { "my", "3997a0610e684294996a81ab3886581bacad064b01cfee99348ec25c25a479c43db2d835dd79c5cbc2d020dea268b6ee7b81493075ac3ee5c10731e47f73aa3b" },
                { "nb-NO", "dda58f5efef8fbf7ac5274598865fefb15a2855e78ea88e8aa82941402dc97ce23e22e4a5d5c787c5ca8888da304c47955e524eb30a2e7eaebdfb32c6f4322d2" },
                { "ne-NP", "99070ddfd6bf8c30fc94d2acfb3e7cc3432b602f34aacdedc851b75ef4814f17b406af5d1e0a597d02648e8e1bace20d1be3636bfef74cb29d8a7f2c8f074fcb" },
                { "nl", "3bf5d90a0290c0fad36481f05dbf95bf4d562fc19a15299271be6d5404887456868c3d54bbe1dbc0a821f996dc955c5e3739b5924c6f346daa0020a0af58fd00" },
                { "nn-NO", "d5e130bf33ada281c5fda30b4639bb413748865dc970d64fdaaafc7c1f85542ae6efdf75b9f78f973cb2030a524bfa596e0a006eec03c0b78f8ecf3fc07ab94d" },
                { "oc", "d906475133ba23fc0f4e46529ebe97743130cf0b1088532feb21eb788f086b77afd4d89666f0d8846ab3809e5896da30ecd0a49da8f3dcb40be126c1bc572ed1" },
                { "pa-IN", "6e941a6eca07af1c9aa421890c4f33d0baa4b9ee398869d4fdf07a1dde33a650e8bf40ee288ae0fe2ec3b501d39ec0458188e0575b8d9a5a5282c570155e8309" },
                { "pl", "e4be640a474ec63910f6ffc1d600367f3558e84c6b34a93daf19a8425f1998cacc055b0d37984ac82eb1ed8d4c56674d8d11e28c90e719203bf4c1b2dd426c65" },
                { "pt-BR", "66095da0b0af501039549d23cade5f0d8afd79a07511645855b10e811695084208b427ede2f0c94e5abe1bf12af0603c79b0844886f045bb1b053bf5fe6a8653" },
                { "pt-PT", "cb2c2f0e9e25b21cf1803daf8d6c2d184146d68858726ff41eb324d5affd222ad4bf12f10dacb03593d09d7117b70c1d09dde09f18ab81e3ee2bb64f55e87a4f" },
                { "rm", "57d657bf65f0807b6697b7e4d6eea4631c173a4d54b47e5a7423ea514b45d89d828af59004fe5bd719d888a348df3a451692400a2e89de3ce348cf503233bd1d" },
                { "ro", "dec5cedc6511a22b58b628fa8c8f00f66e3ec3333c1f4a7f0aca636dd578abedc10dd66433e7a46d20e4e67f3f9aefd0aef2d8d7a8618cd29628e8be8d71d418" },
                { "ru", "4b2bbf1d13c8b997db4aa38e795aceb05c735bcf155a2d568b1a7c9595ec22213fb9feee5a841b097c65dbb47aa7b4b36ca0130c50b694c0804c7af124f23099" },
                { "sat", "58c220f73ad9a7859199a1b9675396f1f8a375ee251aaf150c43af87299f6da502830549cd470fa6195e2a3955d5a8e229630478cb56c8ae549547c3034ec6b7" },
                { "sc", "7ec337950eebf34bfa50787c329c5ee466d5b54dd26df7ecc9840536a84038e7e351c572a9c65affadb0cebc38c48f892d4add0b21423c4cbdf9b3f49ac97f1a" },
                { "sco", "a840290730a21b621fe528581131065b2039280c621b8be587cff981d634f6ca6afe2df4cba7c1972850e7dd45a383a8010cc800e3b0864e1bb91cc551b7f22f" },
                { "si", "386edcd0212786c21d0d81b50ae9f35ad3297f629619f161fa6b3dcfb0619fe7f386b41b8726fc2225fceaa291f9cfe6dc21dd7ba1c8664d04de0d811bc8cbbc" },
                { "sk", "c2f9a0d1484338142d9c63a4201a004b7cfe33a875c22650cd2f1950087ff6face46c8a00b0d9e5cbb0e2f1ee25c990d338a356166819fbc99e3d78eda12341d" },
                { "sl", "5378796bdb302ab34545b0cf572586e5954309fa938e3d164901bb4186bdf1d227a6f01012dee252d1c18d2082fe17927b5e0aac91ab618eecaf7ebaedf0c463" },
                { "son", "9e6ce9bb96886b23fcf2196fcddf6b95c64c7db41a5b5931adab8e2b1b609687b4323776f29305e1d1c6ff1901b0ad085454cc88d978f43437a5fa2a436589f7" },
                { "sq", "4b3843c515f221825b178bcb544d3157b378df7db5f095c465f6343e420f7a7393acb6de66067396354a084467c578d33d043ca252bea038ebfd4b2177bcbbcb" },
                { "sr", "5c657ba2912c679416383732035fcb19606000edd5999ac50e221cfae3a6ba1c407a84bbc82ec7c82992e14a72896af7d840aacac08f8ed88d653a32d659ccb3" },
                { "sv-SE", "9514068479b19c0edadeb218c776d6824d85dd1a4ba0de543a22c0cdf38ff8fcd6ecb01a8c84fe05ec8298cd608006f8078e445cc422f565f4c963482c8274b3" },
                { "szl", "5b28876f09708fa4a8c89e53f5b651e48f77812a0b88982d405e4cab878533195bcca15bc4b64ad0fd672d2e9039afebe221394f4ef126d6cc5201f5d36e8def" },
                { "ta", "5f056b8f6a76829719529e5f215b16117abbb3e66826c41425e5b73983714735aa0bae6b0fa21f9beef0801153a463c91de3ba95211ee9f5db2d18ff5c941b82" },
                { "te", "dea58cbdb386936d8155bc8ddd5761b3469818eb407128031e516f69032d0085a0bdeb1144ed46621957e4c047050f93f41b3ca37a5ee0c4eff58ec4a205844c" },
                { "tg", "389fddc9ce21b998229ab64d28df90d636cba8ea2df922d70fd97c6db65950e7972c09eafdfbbfb782b89b4618a4e62cf1e878e613246f24dedd72be6c08c0ce" },
                { "th", "03925d669d86a6d2f00cc4baf01f92368bef343ef0ac7b7e81d3c002b82c5f17172a7fb36c66999a9ae06741d17418e965df65ffe7a10f96f5a0c41595e158cf" },
                { "tl", "dc57bcca92cbc0023278d970b0b45c98cebb79422a9cd261e08457f67a29ca65c599ff0a88a059360e150d0b8bd239e80f9ee636559396449fc21a4f2ad55b98" },
                { "tr", "4bf53a2dd9d9421720e468e122f8c9d8007bf6d89c3cf888364f15a1b448cd2e348387e71fc97d8bbf3adadc3bf01e2f97e532ccc34d2d4213fe388e9015ab3b" },
                { "trs", "4e07c09c964b612facf64fff6058d703511debbc75725408b9310928763ae45b919a8eb9ddc630fd3bfd94f92bc18454e5539c736c4930d5650d5f8de0319e32" },
                { "uk", "752227c30f19c963f98832b697e46a83bcfdb677023081d67e3600e151c2eb0090b4cada95feaea91b0b2f72378d84f6585a947ea3d40b524f398471a682f306" },
                { "ur", "d83b6838455a7b85a217e7744ad7a54aad8d1cb6890d812cd33284ee47e4dcdd695cf98786eba13b300682c291ccb8e2d5f82a167ed8da111c7fccdd867122b5" },
                { "uz", "702635ed23da484e226b948d3820e01e9aa46e991813a849739ab73024b39b51032f0a8655e25077dfa13c5134f85cef8c874b7f33d7f197a740771114c00080" },
                { "vi", "d21620d53000bd341ba985bc3521115a1c927b43248dd53a29f0c344723429381345aa921c6a21947b2b251dfe273f6b5871968b193196d954e3912427b1a27c" },
                { "xh", "552074ae9112f28f8957150c744204c5fc698079919f1fd7d2fbb383152c792a8d4bdcc89d4f505a3a78e60f2061eb7137f28117699bfd755ffaa6e37a1693ea" },
                { "zh-CN", "e62017fb67541491d32dfc0ee4b8553c11c470126f0fe637829a71d2e1b19ee8691efd4f38f7b30b2d6a0a4e78e59bc298136c3cfbd40bc8cd8fa2d02a706582" },
                { "zh-TW", "0acfbf3119925e2359b88e66f1627974ef30c11dff803f79a750b489b97e19a480b000d8e2e7edbf1672fe03c515a4391eb5937964f4ae5029616122ee29a317" }
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
            var signature = new Signature(publisherX509, certificateExpiration);
            return new AvailableSoftware("Firefox Developer Edition (" + languageCode + ")",
                currentVersion,
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x86 " + Regex.Escape(languageCode) + "\\)$",
                "^Firefox Developer Edition( [0-9]{2}\\.[0-9]([a-z][0-9])?)? \\(x64 " + Regex.Escape(languageCode) + "\\)$",
                // 32 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win32/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win32/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum32Bit,
                    signature,
                    "-ms -ma"),
                // 64 bit installer
                new InstallInfoExe(
                    // URL is formed like "https://ftp.mozilla.org/pub/devedition/releases/60.0b9/win64/en-GB/Firefox%20Setup%2060.0b9.exe".
                    "https://ftp.mozilla.org/pub/devedition/releases/" + currentVersion + "/win64/" + languageCode + "/Firefox%20Setup%20" + currentVersion + ".exe",
                    HashAlgorithm.SHA512,
                    checksum64Bit,
                    signature,
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
        public static string determineNewestVersion()
        {
            string url = "https://ftp.mozilla.org/pub/devedition/releases/";

            string htmlContent;
            var client = HttpClientProvider.Provide();
            try
            {
                var task = client.GetStringAsync(url);
                task.Wait();
                htmlContent = task.Result;
            }
            catch (Exception ex)
            {
                logger.Warn("Error while looking for newer Firefox Developer Edition version: " + ex.Message);
                return null;
            }

            // HTML source contains something like "<a href="/pub/devedition/releases/54.0b11/">54.0b11/</a>"
            // for every version. We just collect them all and look for the newest version.
            var versions = new List<QuartetAurora>();
            var regEx = new Regex("<a href=\"/pub/devedition/releases/([0-9]+\\.[0-9]+[a-z][0-9]+)/\">([0-9]+\\.[0-9]+[a-z][0-9]+)/</a>");
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
        /// <returns>Returns a string array containing the checksums for 32 bit and 64 bit (in that order), if successful.
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
            string sha512SumsContent;
            if (!string.IsNullOrWhiteSpace(checksumsText) && (newerVersion == currentVersion))
            {
                // Use text from earlier request.
                sha512SumsContent = checksumsText;
            }
            else
            {
                // Get file content from Mozilla server.
                string url = "https://ftp.mozilla.org/pub/devedition/releases/" + newerVersion + "/SHA512SUMS";
                var client = HttpClientProvider.Provide();
                try
                {
                    var task = client.GetStringAsync(url);
                    task.Wait();
                    sha512SumsContent = task.Result;
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
                var reChecksum = new Regex("[0-9a-f]{128}  win" + bits + "/" + languageCode.Replace("-", "\\-")
                    + "/Firefox Setup " + Regex.Escape(newerVersion) + "\\.exe");
                Match matchChecksum = reChecksum.Match(sha512SumsContent);
                if (!matchChecksum.Success)
                    return null;
                // checksum is the first 128 characters of the match
                sums.Add(matchChecksum.Value[..128]);
            } // foreach
            // return list as array
            return sums.ToArray();
        }


        /// <summary>
        /// Takes the plain text from the checksum file (if already present) and extracts checksums from that file into a dictionary.
        /// </summary>
        private static void fillChecksumDictionaries()
        {
            if (!string.IsNullOrWhiteSpace(checksumsText))
            {
                if ((null == cs32) || (cs32.Count == 0))
                {
                    // look for lines with language code and version for 32 bit
                    var reChecksum32Bit = new Regex("[0-9a-f]{128}  win32/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs32 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum32Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs32.Add(language, matches[i].Value[..128]);
                    }
                }

                if ((null == cs64) || (cs64.Count == 0))
                {
                    // look for line with the correct language code and version for 64 bit
                    var reChecksum64Bit = new Regex("[0-9a-f]{128}  win64/[a-z]{2,3}(\\-[A-Z]+)?/Firefox Setup " + Regex.Escape(currentVersion) + "\\.exe");
                    cs64 = new SortedDictionary<string, string>();
                    MatchCollection matches = reChecksum64Bit.Matches(checksumsText);
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string language = matches[i].Value[136..].Replace("/Firefox Setup " + currentVersion + ".exe", "");
                        cs64.Add(language, matches[i].Value[..128]);
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
            logger.Info("Searching for newer version of Firefox Developer Edition (" + languageCode + ")...");
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
