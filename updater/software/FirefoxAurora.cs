﻿/*
    This file is part of the updater command line interface.
    Copyright (C) 2017, 2018, 2019, 2020, 2021, 2022, 2023  Dirk Stolle

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
        private const string currentVersion = "120.0b9";

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
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "874a8c067446b8c81b65fba53521a8b4838e27afb20869e3e4a42bfa726989f81e9b701097df4afc8d5e62e748f174cd6e2d532177f8a0e66707405cc9180e68" },
                { "af", "dd5fc6459684fd14d56ea2ef5de2d8356766eb9cc9a696124f776b3f839cafc87ca1d90e39c1bc0c3a7cda1108e7d6871ea4c389612983263f55eb584f14f577" },
                { "an", "14a3962d651029f891926b1d7aad16ecc357815da74ab0cea60c05170f29a9182093fe9b7691df7db9a60939e0202ab494b40cc2a8821e64efa2ab42e088b97e" },
                { "ar", "9183cfab4f03c4b02e33ef1765afa54e9ef274876fee0118ab4141c5ed6b721aae3ddafbdd88f5db2832efd063725dfdf80645cf9990f00b295ca69a8a9e96e6" },
                { "ast", "620e6e858567442495d61c26978859fe570353fc7ffd35ee89978fc122947eecd55bfc1f1e4752c28b4907b0c7561b02e0e4605871320c5a9fa0461bbf853c5d" },
                { "az", "5aba86f272cd63bf8e2202e8417749e99479f5e07dadedd779094e2da115e5e3edbcd1579a6cec5435e10292491ef4d5731ba4ecfae37ed3c5cc33d8b14a9a81" },
                { "be", "505e1601cef5c624ccdd7e1aa641b563346d94d5bf3d6e925b3af35e30537c77c3f122531037618f031887a3b6ccb6aa9ca95e68e82d85c9aa39241e6852acbe" },
                { "bg", "f080c538e58e8c0a5180b4deb8cd257f93b1dd5131efa3fff84683ca26ff1617b41d437936d10d83691ba65162e52575566eae06e45e722916f0074fbb43bcbc" },
                { "bn", "12fa49e17ec06e9ca874042c4be3feb267558f2b442665e9c121318457e28606da706341c0eea11ebd8b51127ad7f58630e2a0fe062d449e63b2c2e000af1c52" },
                { "br", "625cc31264a746debe12784bb1658854dbd41e892f39e61789cb9fb9d954105d1568d7e3bde301d1857afd12c0de3ccd89cf94994ee3191189b382e4a510fde3" },
                { "bs", "e6faa1a2fe8ab5f019cc18b36c5d56c7996182121f9aa1998443fe59f6a3c290c3c4b51a827678b30dd0943c8147add2f3581bfd04f17bb3cc4d1aaea33e37f2" },
                { "ca", "8665d779433c116a7e99ea0d69d4a9eb3eaaee5ef00989f24c2e363537628da852599fd90d5d821f6f72a38487512c5bce2cfef70b410d177a27ae756ce7dcf6" },
                { "cak", "7f7c45b854a49787db1fe60e09eec0ed900ed6f0d358c12f0c70bc226a1028de9075b6d1ea7e974215eeea72a7b70aaa5c5d0eb1e0e92b7144c48d5cebb2c3ab" },
                { "cs", "e13da87f3cdfd5b0206bdc8cd536c4cc8b91c72f43c620e4018fbade4644f8de774c74348192dc144b6458d47b07686982dd89d294c379770d37073344c7d746" },
                { "cy", "ba7df2b103f888667570e571b384f7edb0abab2a7af8c4fe4bda12d8234d32a35785b3c4104b91ad6d2c11106b2ed70587d1191ebafa3120c0e43165e91978d1" },
                { "da", "93d4da9482ed4814ecbe39bf91e145ac667d44a7c2024d643a2d60fb6a7aec79b3ba129f1a1314acbb81ff070cfd6f0a3e080a7012deffd8eb9f7b0d82a5a01a" },
                { "de", "e2e905009ae5bcd1a8b796eeef0e9bf01b332141cc7c72aedc02ece5c17343c7b08ffd6c4451198d7f722c2b46b1f1717c57a5769dcc92f6ebe9c532fb2ef9d7" },
                { "dsb", "7eec7209eb97ebf2464659e9f562ff4ece23dc8316151bd5ed9c709e3ed94299a60ca3b49ceea6dda0d23892489b95a3a7b9c6b567b0ca6aefcf817c61bafbbd" },
                { "el", "1e7e37eb2051415500989c9ae7058ad93a95f8560286710fc9540e7405f119175e74bedf1252272320763219812b7f978c1ca3bf0ffb1e62aa7741e8d86d81cd" },
                { "en-CA", "9e54612008b11b4200f3c9f0568aafb412b66b7e39e3b579693646e05da71320f0c9b8c33eefc29633b988c763dce54b2f5b2e8d77b8cd6bbb234926e79ac448" },
                { "en-GB", "de79cfb215e684983244eb804ed304dd7370e9d8ef492a0441275385dd61da7f7849b8bb463a9642e2ac9336818d7b3af558ccf0689fd5053f1ca586b4cbee42" },
                { "en-US", "7a70ed6a8a4fca53cab6282c8a7e8eade26bda619da60d3f088c70c80f5bb9a8378473346a5c7201210ae6b6584d80bc0eaee8fb30409d683542f04d83a6d0f2" },
                { "eo", "cef4a6da11ec33bb9aa3ec5b4a34c5e43853f41f97e31c8a49947a9c42a08649b905af8605f5f43cd9e4fb90893413189033c1135b4d9611e8794690d275c7f6" },
                { "es-AR", "8df2183f0ba03252defa76d9d9036520b523fea8eaad806169437e7685ca8cdbbd8a3740a85e584dba265265a130aef9348fa28d072bcba01355f36678b33598" },
                { "es-CL", "d49d7688b23009a61d7807b0f2a4278f299714769cc41453d7e28e8fa88e73f058af1e7d873a83965e51fde4d50726d61614f8c0b9f660fd03b1400a3040bca4" },
                { "es-ES", "95023c14eff77f1d4370602f66418f050792e72ea973af11c28d89d543fd5cf6d3053d2403afaec5c08ba5bea0f7ae3cf30677c1bca74157eee7917d1f03a896" },
                { "es-MX", "c71b0459b5fe98d65938a6e8bdc49ee36b8b8767bde34c0764053ec30b590febd1832f813af792fd50f438bf43b38f1bdca586d38e35d51231011347bbbe8a19" },
                { "et", "ce00bb2fdf8ef7d7064e6742d49ded8edf3b6971f93debd03b97ed5c09cf5e62e456438694e19de006ed90a0a409da998bdb240887b86169298e4807eeca3d21" },
                { "eu", "59431458716e25edfc82a37bd44b573b67f5df51f07c016386514be36f6ae2c297dcff157411beded681c62ea952dcd47222200cd04de3c7aab794f048a6dee0" },
                { "fa", "ad9d8490733c9bec9f8ee5ee5bbb3f5dd8968e54993b768ec4d80d60735d943326701a06e0dde7a8190c2dbb0c455258a74f28c37819312fdb43148973d0371d" },
                { "ff", "780bb898593aa5edb6299b42d1b389a3c396e81e0db9ee613872c6398bdb568b6cc346a8ecebc0464d81ef813dfe4e73a759ea7fd0f2d02c8a7c28e2aa11ba8a" },
                { "fi", "e16f5a9f05217ff0100535e183462cd6b8d2bd57528e9f3597ddd42d072a214215d64c29e0f08f52706f5280680f96a08e0146d9dd816f71b006aa897a1c6432" },
                { "fr", "f2b8fa0014d91496fbe863ef43b822f8d2918c8e4d90306a43a35e8ea0c6030b8764d67abb3808d698de246108a3ba964acdc3acfb0b4d7b8c28fe669a5948f5" },
                { "fur", "3ac52f19c7a5a12a573ec587d951c7a4daaf65a0aa9288249291c3f51bd557637919b942c540edaa6e2cc12e201486f867af5a4d4a3ed8a2a5aa8beca7c3caa9" },
                { "fy-NL", "9eb90652539b5e7610711c22641a67cd0e3a7c45415debe0b116e8a58d344471bee9e17a1eb7eb3856b088405afeec4886ca194f0552f14af984dd012ef53a36" },
                { "ga-IE", "83186ab5b29421794f75e7086468104804206b99d964eb348b2721f326ba00d231a162713074fba50e339963c5e31d07a7910e9e6605731c6a1df1601838f26b" },
                { "gd", "092449e09946e9fb820fd38187e210c1da4a2feaa4dedd810422ecb36c1ddd19beb79b9d10729e5fdebfe53214138e27ac98a4099560e81f72e90f7eb650f8ef" },
                { "gl", "e6439ae1118abac61c06e8fb981a64e12c39fccebe2adaa2a0b08be8dff26f1b01c90ce5010727923f47a2f53a2592ac25ead9f8430b9bdd31f157e9e52c0d51" },
                { "gn", "7d01a4b32829bbb2ca7769475ab77bd9dd973e9d8631d711610295bf7850e4fb807ad0a090202f9bf0d27f729edcf53f818adb4eedd829924b17b6a3eb25a545" },
                { "gu-IN", "c81dcdd37da864db8dab4619f84e8f0157d4eb824375735dfec6464696c2356a9fc588a2aadaba5f84e90388b6ecaf8dc5b82af26b6caed0fcc1f2fb05212e0a" },
                { "he", "78bae8d92fe2e3643f73307a1750d40d1ad886e6f915dc5b8a25cffb0c99e77f06ab7f0d0573398c15712b998cc6c9bd5cd9debc3ff52e9b9ceb50c3554b35f0" },
                { "hi-IN", "e3fb0985269bfb3ca3990aaac5b783088e227dd442b8588f4e17a1c7e6a50f30504df576851fd02cc4a9b522fd85db2226cb2f894846bf753410eb5ed02e0083" },
                { "hr", "72ad3fbf49bc103e5999413137d79c8cf21ae02927914115106d618b18579e9eb00ec745df64d48b9970e9d157e1ee37bb265cb32bbac0cce3c0fdf860f6d1f8" },
                { "hsb", "b682359791ab52a0b6f68cb9e92ba61b3a004404345d2099034a879bfea5f633f3507045f4498e026c48bbc4daf8635d56a3ccb27824963a05aa80c2064995e8" },
                { "hu", "893761c978cd82efffd63f19a921c07384e966202e5173f856bebc30404d9dd5048dd18a4d0949ca85e3be318a9241b7d1e1ff3ae4d779027c941a9b9a2c3ae3" },
                { "hy-AM", "afef77196de56e44e239886ed004753ce42cc741ab0503e43f0828bad7701b870aae9d6a0fb19d6eeeb4105459f9a915c5f3ec9535464c16e9779aac756a67e9" },
                { "ia", "a84db90fa1f6fa8af149ed170e8ef0daf11670ddd2f3c869b5427049c9d09b6491ae558c5a4e4d66e060f632bf24c5cd5c0069d1e307beaf89b2fe1738bdc69f" },
                { "id", "63eb78c362bdffd3aceb6ed1edb306ec81b1ac965866f53498f17171954043e6ccd3e67c52f520be99b96b87791af3add55b3c886e39ebb45b0e198c15863535" },
                { "is", "f6ffb862f52c603630af3d820f2cb25266353d18869de71e102a6bfde44943a5f9431df95e6e0efc528607391286e46291136b38218dc3f5862ec28f235de40f" },
                { "it", "59ac9686580f5031505a41bf4b4e7db5959dbf00387cf442323d07093569d3d1b84155c0f6a8bda4304d9f178342a4b22362457b6d7321832c8e2f1957b6306f" },
                { "ja", "33689cf1cb07a5b96924fc17da5e29887b01e89e30d20a6fc4a4fb03d64329694778f960daf6f47cd57640cca9932bbf134d69ca69a6b3f7b05afeaa477cc142" },
                { "ka", "4bce15327e0e2c6a5ccea3c46aa8123a2e91bc881b46f5aa344393de6392d2ee17d92f85db675b423aa9c424e5b673bf5655121d9ca9c17a3eb6a19bc466fdfd" },
                { "kab", "f4f0f54a2492cda6231b9b06de698ca34ca3bbe0b10a08e0d5e4d26e6664dfe38763dd3efccad3607127f62d16de8ae6e8fad02f532664cb9c4ab1eb8a3376d1" },
                { "kk", "dae7527379d23e8eb76d250660ad8f0b5169e95e7df1849f54fa721f68d24c465b3cb386272cca0eea8a1fea1dcd7f0ada7b70971965d2467a53e104a219609a" },
                { "km", "c0bd37017e13b5fe494b517926df8439660a68c7fdae7410698bf07af5f454b2d80e5242e4f8a47a1447f2c7aa6b74f9ebf442daae39f03666df00e1568e2373" },
                { "kn", "1e44864bcbff73c3d6508e31f7cfbddc545ce2f58db768163fbba511ff3ad45102a875aeb4c5c26415b2c45f366e77c9809d071f92519bd1a159ccadb3206fc3" },
                { "ko", "9826f12efbadfa33ce68de3a13647e21268f2ad50be05786bdc7f2d442730f6aa18179baec6e46741c482480a0a70142002b9b1e28a404cc69e5e5ba40fb9fbe" },
                { "lij", "dc5af03bf15e28517914dc93177e17be4d38384aa48c147bde28baf6dfdc07db7ec488f17072cef7a75ca7993fb7efe0b0821d5fab16b0fdeb8afbb72463027d" },
                { "lt", "b92cf37c28aedd76f2cdd41b06d89d695dcb573134d51595233afb1908bea65e136034c37228de288c3a63c50288e04e166ea911d3bd079f38ffc909ffb7ce4b" },
                { "lv", "4fb2699f11f0db33ebd1d32992c839fde8e8894834071d43521718a17aaa9874311fe825d84f82a3445d2725a27518ec28f2879b30cbaec07ae06984f8f6c102" },
                { "mk", "83a269b492b4f47e9007f9669a154da73f5395646431b063775811e541fb0096ccfafd3fcbdf94ba2be7db5bf9c02928bfd8a276e72c673ee207340c78b9654c" },
                { "mr", "71deb72cab4839d4f74f91068301e68532481ca3dce0757d2462e68ae325c94bc3da7fe89e910c855f578ca5da94d2ea1b0e0638a03248f21d47ec533f7f78ab" },
                { "ms", "2ff40e5226d33b9f5309b10c15cf39dc9d25ec0eb0e375ba9e51d284e458ce1dec70d73a1b225c82a5ab44608dc2af6173b1dcbf1fe51a15b2a3737b775f4d45" },
                { "my", "9d029c124e347071bf1db8107f69cf445bb13910daf5caeeac103247073eeaaa6257efc18f65bd6ba90da857ef2af34fd34d47946a375b44db022d295611df23" },
                { "nb-NO", "2ba1e6267ce968880664301aaa6abaa5b74caf2595cd5295d79c107d858573f4eb8a3790dc42204475c3629e0a7c97c8ce101f100c30ec870bc52e34710c117d" },
                { "ne-NP", "fef42b982164dbe303dd22d777d26f2cfd6e0fa6c16e9729fa47e166f1e6821df1f10fd33df820e387e181ebd0032804fcbb817c42520a88b397fbb9f6ad1d07" },
                { "nl", "fb589311ab729cd7ef7274e33443918f03b73a7399992084c2066129eb73803718b30841f13030feb560a06a619b74f15e1b570bc350bcc1d6d7323d94d0460c" },
                { "nn-NO", "c56aada2f4c4215d045d9093d3986d4d4e099e404120255de4a1df2f2ba8167141ec73eb4fee3a217a971c5e087fc469e3058ec18cc79f497b29cc98c96b9a33" },
                { "oc", "75ac84b7ccb5eb754e8c170967eb57c1ddbec3973380ee1f24c11e0d905f54756b224a78cc1797f851384d0d39432e023294916d246322e606533a24a3076352" },
                { "pa-IN", "26f40fe07c1ff991422894691c878439f2191c0e656242f9d7a3b374e82bb2076bc17ca3fb62fcf1386ed9d271dfbaf148ce6a0699e47285670065c79d38a3b8" },
                { "pl", "116f4063d4dc56b2df6a1233dcadc6967e86ccafeb21fc4edf296fd112d5dadb6dab8b8e746a7123e4a060b259f82f5765c9eaba6754b0465f65d930accd5986" },
                { "pt-BR", "ce7f45d18a81b94cbb01e544a335bcdd7a17d84c85bf06a3059ad7da858451bee9f4bdf9a2a35ed9e2f213beae75cd65dc4322ef0455a6d22159146ff487afad" },
                { "pt-PT", "ad8380fd029a7c0f443e2fe4ef98d96c7948c55808cb8121087a5de8720ac0cfc12c3e872de869407b24ae79cf51b8e56ff0b31bda4247fa8e1f75c4bb42877a" },
                { "rm", "fd0eb483625043f5b2a2d68315c952f91277c885c908c90660e88dd4bfaabb000345846fea7dbc605558b5d783f386dc785d3de425c8046ef5439b89e0a0cdbf" },
                { "ro", "51df2eedbf1e2fd68876bccc8eb54a6791754c887312334ec87470c16913f8475e728640739a2d8eb0210d98adec5dec556b2391fa72034727a6f151e9ab6407" },
                { "ru", "daa8ac3d289f278907df612e7dbdd6cc3048d68a5ee06398d7d2653fc019503a9cc459975323c12d80cd77f85aeaa0412f48992849fff090005d12153f3d795f" },
                { "sat", "67b7478fbaff708cc1cb33f25fd9c30468d7041951d625f9c06cd82cb55aa7f5acfab9752c70c20af1f753176ef78542455f35e7bced25b8ac3db49a0fbaca6d" },
                { "sc", "a3032ce1bfd4a9e59099f692ced52eb51e1cf3d9164aa19c7b043c4d822636fe950937c7237d136f067021eb76eb71fdc9aec4f80c3d90736b2a3aa631647562" },
                { "sco", "895d2bb61aed154b1a156360c85a7954b146719ea74493a01d05aa9e86cd3e1c9b08d3c5be4e64a9b9e6cf7ada3a8d6cb0e9a79226ed274f96ff9b417e9d29de" },
                { "si", "1c69ee2adeed47c3accdf0acd9471e81d8a47531695c743d9a6a99dfe100f3713e23e7c56e7279ce28e3e04cf699a1843a70a9ddef8a70581fe121827904aa5e" },
                { "sk", "2ea0811e4c7d66280a3a31b29f99e02318f9bd27b6b252ccad973ad696699b67a74b8954af8f4f59f45dc04d8352206ab4526d823d02e88b1702fbdf099ddf8d" },
                { "sl", "aadd478e8d6169b8f24e3a0eb587439f7586e3c96baf27cc74edbd5679b18fc3e860beeb0a2ac0db877ef751a664e0f1647977812103c0ba488dae0766f31d2b" },
                { "son", "fda5809081b4e09a0d28f16eee202754c7495bbb9cbabd7eb70369d828b99519ed5310f82f2d7714383ca24280f48026bad80c430368ce95b4163adc28f029f6" },
                { "sq", "dc12c15853d6eb395cf4e31d86d51910f14fd4759ca62faeefcd3a77ef19580584ce77bdd383d3664f9680a9f1c9786a4473d0e78b492e349b5ff8d196006c90" },
                { "sr", "efbd733580de1fb03728dd4a9992f97966f725ae2c2527593166192ed19fd32b7a372196cc78e2f8a9debc4c0825191aece66cb08adda32d477dcd9b8127ba9b" },
                { "sv-SE", "e93632aa7b764c5eaab2737c8b6c4faa1dd6f4a8bc3f6f652c3c082de3501ae8cee1356ce2a0450c6311cbd0af744a1f815bf2a66dac8e411eb4d08c9af49fca" },
                { "szl", "22a6727005c1e4b7b51b238a84ff8f3fe6324c079cc279dd7611da255d1369878d5523a34c3948b9c4612f1af9947dc0a07410ddde6668f96908babbf110c4eb" },
                { "ta", "aa1201c862cb288fb962ed6a788943b931f005f4beb799d5d1c4e548119fc8027f505717895c06233aa7bb13ecec97c78b549d80e8aa7fd2d994e8f11c72b07e" },
                { "te", "b1835c4d10b7207a86c5337f575d93ca21c2bf2651688f378756328182494d0f239d6cb5c5bfe244a78a07da0b33c3aeb68e0012f8cb54a8edc1f60485d21d68" },
                { "tg", "fd2af1409861147bab904282d0fcaeeb3db327ad210611fdc9fd43e1e4a1dd8fc81baba16d875d4e0e846ce577e782ee31815768e8346ebbadaae2ba8aa385d2" },
                { "th", "e53713223f30bc7918e1a4f612a0f124094c0d7099e6f7edecfe30d800da45c28e94bc0eb9481bd92e8b41a55914f7978b56bab9a8655ee3f346c9894103c537" },
                { "tl", "e0ac511fa896958cdedb6bb13be4c3ac075ad84aaa3d2f07d6e1f672571389863bdf50913dcb1d320fc94a587281c2d079df5f9ace463065f4a9b638b972d304" },
                { "tr", "d0e1b398cb7b55fd6c55a3fbd2abb619cca0c6a655b0a6d3cd47fb1dbfef68686bec8772c75a54e9e23b39b51def09dcf0812f8224a71a218092bdf706892f86" },
                { "trs", "693b61d811fb85fa1b15cebc2835504f20a451e81c29ad1e1c0049c1c9eb904e8c18ccf5b69c1d2e242a824e86b9a06fbdf92ec79e6d3eae43d1a938c09d0e18" },
                { "uk", "87607ad58d10e56ab3ea90913c3ae40b2413ea8c028684eb60d033e019725456d5867054d1ea286804a0710feade9829275cb2efa5e70f31847938b46ca2d06b" },
                { "ur", "e910388ea797be87c2fd291cf206e59d47292a3de309981cc42109b3bbd49144459a4252197969a69189c1598ca15584d31e1aeebd3eac5955aeca04e2adbb2f" },
                { "uz", "38e034f40553b81e084bf0a39169112ad9d2788ffe0dbb16b6a6cec93d6c5feb2251f675c5cafd12cff7268f4489c91e12a9d6f7050b369750b10759ab85b461" },
                { "vi", "ae9edb1ae775858a8c8b8ebe49f8b40d6211e461487b74a0d0963c3df007516126c1868644724003783a3c7f48cd555700bffdfdc30b59ca57fec77d7f5b33a0" },
                { "xh", "c012a7f6a37fa112a4c27b92fca2ecde7f223eee543cb57119051d7d11882ef794a53ee1259936f91b25135d656f43d512217ae3196cdaad0f3bb37803243e24" },
                { "zh-CN", "baadf6c58af6030b24499a0376795da87d07511d4ad9670cbd41b32a8f899462cc9c560736f04afa910d60d067f75f37afb4cd50617559550d50c90c67082597" },
                { "zh-TW", "e1c83da8eb7d0b9f77940081700d57f0a859a7a1e100dc73e13f6226096854a6b8fc64e2ea45dada835a836202951b8f72adbc09175f1f6699a88fcfd19cf45e" }
            };
        }

        /// <summary>
        /// Gets a dictionary with the known checksums for the installers (key: language, value: checksum).
        /// </summary>
        /// <returns>Returns a dictionary where keys are the language codes and values are the associated checksums.</returns>
        private static Dictionary<string, string> knownChecksums64Bit()
        {
            // These are the checksums for Windows 64 bit installers from
            // https://ftp.mozilla.org/pub/devedition/releases/120.0b9/SHA512SUMS
            return new Dictionary<string, string>(101)
            {
                { "ach", "07055caf410a36ea4c73f7833bf2aac5ea4be268b66b176606ed2116ce1025e820cdd30048a2381cc7978f74e5ac7a4175357174f6a8685a2f371e5c1a108417" },
                { "af", "3f6ee91ec71f5451f5a4992ebb7b757651b4e2345399b911d6a240999673743f8f0e369594ab5468d49fe0ef20f36f35526032db1c68fad7780f5eb9feea4fa4" },
                { "an", "11c385e8fb5dc34c553a7c54b2bdd8e8a5f3909a393aa729821aeeb0cc984cffe401fdd4e83473c2dcbbb6bcc370e6a3fc2fb50350381d28c1c658e1c759f307" },
                { "ar", "25564b0a711d4ca5211e1320d55002e2fee008782eb2f50e71ab78709564d97f56180f328e7ebf9daebcd66449af31502c2051ef9b029b5390dd9faf12cdf4e5" },
                { "ast", "0daab5941872292fabb85758e87128120b75064a8970d08927c6d3f1ee335c35a4335eae6cf125b1a6694af420a775ae8e7805abf54f339e43fb82969598a7a8" },
                { "az", "703c413ee10804fddb74d1954db92787cb12ce29fa636318aed9d76adc7920676063a57c1ec06baf6c69641839a9156fec61fb8569bd43beaf96f5303ff2b423" },
                { "be", "a60db98b6fe3bfadea1d8ffcb1ab34ce7d64f47debbae24e209e0ed9624577b985bb7ca85da66a6f8bac02acb1d74f4cc565892a65074e6c70dd90ea74772e4a" },
                { "bg", "0cb2cdabafebad1d518717430f41dffb57dce04fcdf677c2dc543ca00c73b00926b3cfa93562ba06b682498eeb5fede2caf74fdd225f07117c01f66e5d2f175e" },
                { "bn", "202f7c98afc26a7574ad3e7e017ae837b25bd2369a58a8fea3fe1f164e5950212a70d7c869e14b99438685b0deb2eaf0df659ad689a51ed9a514a84b171accc9" },
                { "br", "38991d6a2f4a180b4c89b3f38b0c7212b2fbe886ecdb2a5194d8fb57835f494de823d8a176790a37eb63c3d9c2f45ad5589e80d0fe114453885a8ef379bb4383" },
                { "bs", "99e9ce8344b897a2167d5b08c9983ab01758274b44e1e73a64370ccbb3770e1877a174cae195a3bfeb1b25b5b8212239b5c51c1c4007f8632ae2f335429189a1" },
                { "ca", "a89c5d74659c786f0d021c5349337cb98228ab13af496635f89bd04912ca7a75fa196ff7842958f9d931a9c3c98482b07468e90f6e69790e99c2e07353fd23f0" },
                { "cak", "d528fe6b18eec290426a616fb03718e9c2294ab3bf2e9049d9ede9cde200e2f4baaeb8c360565e15a7b0ff6f6cdc36467c2193e116b2311900ae714007f54453" },
                { "cs", "8e2d724f4a7bcd63f7f6723714a7e239911ac9d00974293b521700eadfe8867713584a45116fd8981f1eaaab53a53b97368a7a4667720b0d7dbb944e7b641d90" },
                { "cy", "260639dd25f9a859f0f638757e1dbe8414a6e18f71b33d2f3ff3dfe71ab102244b3941495ce94a2aefb908fe0e696cdf79f6bfca26b55ee8d8762d742245ccd9" },
                { "da", "168417f1c1d98c4f3aead4cdde7ef54636c837e1f3ddc2d26d48a855a4972d457060ad5e69a4f73d0bb9486f490c4ba05479442ce82df4ce7f5572f5d94bada8" },
                { "de", "ab97e159c880588ab93bbe080044aff3c0b4ef3544eb90a23f0b70c8451e20a7abf26265e9cce4fb6e1e48fbe57d55a24da31f0d81d74547442ed09f2c41c631" },
                { "dsb", "13b2762d1799ae285ec949e7be4cbaafa37bc3f3d11b3b0729d0eb23de4e1a8a3fa545b130e4a4eb0cfd5a97b253922ea6e8d87b6f6d04991086c315f9ee9e76" },
                { "el", "4fbfc2b40ded6b1215eb773677454772523aeca850e9c16613b1d9af6fb4687dfd37049ffc77abc63140fe76cf188a369fe1082090904e69ce03046db54b6628" },
                { "en-CA", "45ca7377109ae6933ef825c7fe01acd8943888413ef4f987bd810a14bc499284017df1552f2e21a37ba6708e86a4f905c84f3d6fc5d0499aab294b25ea843eba" },
                { "en-GB", "6bfcc5529a43c2ca1b31db1a4372e2b63ed4a0826c9432c4dafe19c7784493cf6dfd7105e47d355347db8fbdeb10a02bdb5e714f4c72a990c1e7c1302c55b6d7" },
                { "en-US", "e2fbb1097ba6d54fea5cbdc41ef9f818b78bd8613f27cff77771041b7edf3a728023db770117dd24685c4874e11c14bcd04b0ab38c643e79489db4aa83001214" },
                { "eo", "429d6f5a22945a9c1ba539f883365d28c0eafbe7ef9915637ab1bb7dc81fc0a266617b4188757f5e8d25e2d86c0927f45c0d882aa3b91b69810606ab8af8417a" },
                { "es-AR", "98cd47ee68d009628b5ded9ff3a70f6084b55c1b11cf3583462aa1a88af39436c2beb3523faf809377b0b309efd2f11b1df21c5eb947494cec03d0d9e9e6c12c" },
                { "es-CL", "644c7a4b9fd1d1337212d9227b3fafdffc805449444e6bdc848c9288371eecbd50a4e5e991908d8b82445b6a65302d942076052c4c06a294d13f28911209d974" },
                { "es-ES", "9abb8cde068daad1175f32d6ac4cbe153e8e7d6bfe84a6b01079c627820ba27b7bdaa9faa04834e9274c605cf99e46957e43792a54513de864829e9363df883b" },
                { "es-MX", "aca79d28af6e7ceef29ac518e40bb166957783c9c8c7131246ec070d548aaee9d73b386c4ee3a0503839dc0ee3959f1852a9dedfa27f123db1a143654a88d2ac" },
                { "et", "1583c6c40cf891d008810f851a191c1e1aed559584a58a9b61554f2712bf2c53803669e800f8982867c6f36f52d40de038cd2fa5ac4394fae295bded4bf0eca6" },
                { "eu", "2d0f3e5cff796a94d0d4f8fb4882f1477668d2c35e895c6937703507325a2b4d1efa67202334f803b7ba501cd3f82430ee3aa8abf5b30ab0de4ed4d9928c2737" },
                { "fa", "b55fec563422f81c30250da39025c72f2b3874c4b71f9eb24c76bc0bc06a7da87d97a1f73bae9001b5710d79b2c537987ea6589300968566fc2206c3a76ac590" },
                { "ff", "c43887e7e100143c8820f39cc74bfc473b9825c02031f7d78804579fc1969d305150852856e6e7c08f6a893b6c02b92759878678f427acda6a502b5f5e37fdce" },
                { "fi", "c920b972ea5bff941cd5dbf46a764abf4e3b38b76fbc12e654c4ce53217c45fc72673ac53f349192c13871e0c8fc5372d7b53db52f89eef00bdaf968a8ea65f8" },
                { "fr", "24544de4a3f0aeee5f62e93069498a903f11705be1fb0e7810a7bfbc9e49069addb364291390434db34bcea1d60843aea923a681e34e42092cb08a49608e8b00" },
                { "fur", "8b70e2828876b341088fd10e6c1346d4eb11c02255790785d8309f69f67ffcfb9d1831ee3b1edaf8b19de1fff504ff01d99a53b0f29d1106d8e745a63c826231" },
                { "fy-NL", "76cf9e44b86e463d564999e428ca85c429073a7a28900f08feb194a9a0d4b871b3a6c6e3ba97aca003f744a2f2dfe2425dd7f1c9a8cfceb7db54e09ea6a54635" },
                { "ga-IE", "f844a314b1e46357a90aa4ac25e9fa029188d85b8dd801e2ff842d8191228991fc62868472f2053bb7d08420866ade2327c6060eb91b06d39a6291b4b04cb1d9" },
                { "gd", "652d9dc4603b8e504b37a7a84aaf8a5ff5ec91f21c9460888a57217cf3099d5964fd9df5d9075241f8143c208749f5b47094f317236deb501f2c5488d3ddb447" },
                { "gl", "8e03fcaf18e9668978577bb6a38f95db9bbb0dda6c71ce1e5323b3977070aac7bc3e23a8cd1c98805179da5afd6eeb7f5b07fea99a7ca552346fe64d5cc4b471" },
                { "gn", "68b5d2b19aa52ff802386767970e80c32e5fb6911919183c85e4b1f4fb00d500b3981e2a0f70fb246e651163b477d83d527288b1cf99c802ee1ad8e3fe2a7299" },
                { "gu-IN", "1d6176dd0c188f240cd05b3b3aa51896e631ef4dbf883d5d3e2f21ba343686799a8668ab1755bac104839c0526f6445a9a8df07efc59332b3e8d9b05deb539f9" },
                { "he", "f939f3c2b8f4fb0c166e5d41fadac73c1d9110d8829f101c4c529d1084673bafa6b67822e1a43812dec84d265c600e952e37435537ea69bdbb25181211da9579" },
                { "hi-IN", "e31cfe65100030529376dedd7c6e4b9c7336524db93c35cfa9a42b31d6174c43d1c39ee75df980fa881e3ff92e52f866b3c6fafa14b0ba5c978900c68f2405c0" },
                { "hr", "f12798f5262c25be3d4020ade32b6df67d88aa96746585ed44c8282c78c7dc0a4d2e07b4b503f30c91f6695c376a7ca5674d17ce25c23f40688f5919c230f563" },
                { "hsb", "b5a4d3aef25d6f81387270951c1f128a05f01dd77b737b3572d78631f94b951773335fb23cbc9314f89ab67aed334df30bf7f8074aee1f2b6d708c5541bb7450" },
                { "hu", "35485e4b3066a2da59e7f59e68f4fafe2d6172eeb3c7abbab4ea2eefeff4b0305b7f91bc3a4c07ff5f890ce72d62dea685bd7390cd4375ad0d48c52c5ec1df6e" },
                { "hy-AM", "db5e18068f8bf303683b686615bca324abd6ccc5b8628575bbc670dbf18b8ccfc8d4f4bb48d7ec3fd17d1cb466058a1fc8e1e78b1ad818da539011f98934792b" },
                { "ia", "42970c63125bae5020ee439f33892db217731391ba27ec3e4b76fdeb38281cafd56b1edff15cb4e018cd06725115a2ef9399604f5547bc1d4a1e72f1ef9b79fd" },
                { "id", "3ca77464f3de1db44afcc784ffccf0f8094b878d9f9ccb8c0fdc5ea6367be4f291b1edf1b1e598c017d728f44d806fa95ea932692316085bb8751b8dd091195a" },
                { "is", "115b6557c1d78ce0f21334845de427386eab069d3b811d6598c8b7282109fa9e7b24cffe546d3d956768012cbbdf600e08271e1307cc019a7d7e864ec057fcf3" },
                { "it", "eb27e92083a3bc0e5657abc4c8c41a7f5b1429e09bcdb15c81dcaa5a1e038e2b67748c83252565b8b92b8993d82795d495f76baa92ceec503a0de9d13bb38dbe" },
                { "ja", "131eab3c569d5d0336df3a74f8c1cd478ca013d1c7491220bfe191b0a796525275dcd54ba995f7b459acba1b69ae8c8174035cff4fd6422f1d701e15e9ded43e" },
                { "ka", "9be86a62fd6459985ab35b7c765a43070cd3a8f6ca9b63975dcbed6bf7a6498ec01b1a3e4c939be705733491ac239f788a8aa76b9676d9ae7673498f14811286" },
                { "kab", "a4b2a39305b873c7e304adbeb1bb2821ef121c401dc169b2f5af9375e748b88147f7dab6433897e4f0658aeedcbdc7711a99299a77eba58e8ef72d4d6a12250f" },
                { "kk", "a61000e76b0110b36d8c6af489e10ec6715aff9746d776d3d79842471c67cea68debf132ebac8dc65113b5c5ae3ee312ab02d1d009fa2f955032def777c39a12" },
                { "km", "3d2c6c0352923e86cc5b1c782bef9eb267ce23e6e0a8922c295fa1408a0162966ebf487fcd1ecf9be711431a8a305ef6a687374d35a480783353d637323f99db" },
                { "kn", "b24ab65ca83f3d76f9f3b08460160a5d18595ba95f51231d247a262dcc7e9af5734275b73205487bd86ae163e9a446c81341cc834cf6c0c2917086ddfec2e357" },
                { "ko", "4392500072178f2978111f3b0362062fb1c70834cc9e39922b263c2b199db41cebed0e7b206209a8e6f19d0e2f438752d45fe7883f9a6400caac6e5ff4f7235a" },
                { "lij", "8b5d0745bfde439668849f955f57a1dd1285a3e3455a13bfea7460f38cce377eff497c49e8596b3215b31596325bc41da35915d57af11944d25a7ca857ec4191" },
                { "lt", "8a0e1f3e0a8d93be4d0a51f59bc66af22f89e81daca44e3a45b49f3e4ab7c11fd5a2779c627fd0495c1ff070e38294f1a9bd842c71faead1dde7c66c391dd5a2" },
                { "lv", "498a2fad29f9e813af386b19c91dbd033af80c9f72cef93028599077e08a268e50cc051fb1b15f05e731f8482d19a592a075a30cca0d083da248684e24c92b42" },
                { "mk", "b806f633239c4b2541e6fcf50ebe3a0f847ef0241ddd29e930c23101148052ac7d413875d9409bebfc7451de7f85c18cd32d2c558a2e6fd552cff7a7a338edbc" },
                { "mr", "85470ae8d88f25d5cda02e6f06df5bbda6a317430b0e12339819edd174e0598a13833934d64cff7fe9d2e3efa6afe3ce3580656eefb3266e5288429a84a88834" },
                { "ms", "1c6f9ca07fbf045865b251a4a92ab638127daf34d70b981e0a56779470bd209e9b68054c5b05cd48204ef63ac0e3b0db80db7448dcca4949299de6b7b6e7347b" },
                { "my", "10ab584713d6237f24fbada25a29a2ff0f366769405649bfe4014eaf89aceca84d82d04e4a9357f7a58038511699614770ed6f7f0befef7f915fd03ef7799ac1" },
                { "nb-NO", "2d4c6752fc5f326ecf5922446606d7c4ebb1094178fa803992c10f933996d4aa1e14bb1a85daf7e5821fc09eb75d49091a248d163a92e56e524423fff71e2c47" },
                { "ne-NP", "21c1d79853073e6a54d51e2a120913e7175d033c854d772b537f2b7d54f4f10af945b76b1727aa5f0f99480829e1c676c533bee4e83eef64359d3b227aefd9e3" },
                { "nl", "c85b583854d402cb8cee30c02a3357c220025918e400a05b803df0558a5f0fca2f1a6c97e4aae1684ff91e4d18a08c19bf91d13cecb80cbc166660a988ee9b11" },
                { "nn-NO", "9eb77b6949759642fc9d4ad907da2f4e812390d5ce2d04c31864d650588c6e0ba59ea717f2bf10afad1e178fe50979c620e234d923172eb7f2f57fe51cfff7cd" },
                { "oc", "dbd4457f335ed2d90fd0d5aad711bec61f734a9830493985aa1e70b3488901a8c2dc65ffe97a61f5906074745990d1a12e301748a80b534f1e42c8ae8901803b" },
                { "pa-IN", "2f0cc92ffba86e2ce22aed0d4ace4cd36aaf3abc7b14cd85c3d87578ceb99f0aee37206c7ffec665b49a21cdd839bf1bb6a1c3d0608cf29f3239909725dee42a" },
                { "pl", "acac154598a779ccba05e8b20b5104c059501d4ff12ec3bef408e07796bb8c07cbb538d3a2b76f18b820737f24182984ccc4465e92bddaa8a8c3925b448030dc" },
                { "pt-BR", "cfdb85cefa4fde5567cf39a7dc95d9eb997f939bc65791dcc98c79957837c35092ea0a7441f09cb946fbc662e0ff8401a29ee959d4a74f1f66855bdc60fb6322" },
                { "pt-PT", "d64e178547b32ec8f5848f0d679a522d0876db07b531894b230ccbf6115dccfb4e42bd4a6d46c1cd2bc7cd99da557d3284d5980a59438c4b65f347cef71d4d7a" },
                { "rm", "584eb20fbf0b91d1c84c5586ec0f68939b3b27d9359494f3bf04cd529d3003eb5124b07dfda3abaed5253e2d29c132fbe6a2249c82c4bdce7519cc1aeb9a0068" },
                { "ro", "2b2d0d3168a8083769d20d911abc5836c256ee82800ef277fd87ecb8979991557c537fff398967d63bcae608b399846d53489a930616f48a245d51849f122c26" },
                { "ru", "95d92acd9fdd2624e0e00e467edb99200158299b13e41d7dda5ba9b79abb547664a95b5f1eb40b8dac6cbf9ef4a318b42e6764d5eac8fb6162fa7903e82adb31" },
                { "sat", "fe99dff3499d73f9c7867f552237466cf4bea8dad93f8ab0950ac2ffcf16587c16f3c2dc3b3c1e04f5ebadb6fea85a6744177c80452d3a88efc3fc137f5895e2" },
                { "sc", "8197d07b13c24f47d3a86e6e7fd4e2b47bfed098ce731146bea13f79b66708165f69326e11c07a908c3d4147457b609cc8db33f5cacfb93e4b66b99118b4a4c6" },
                { "sco", "01fe58c2bb55a760f83c381a1b999354efa6308c8879f3c99b452e595044faa0bf99a327a6b94247f00d8cf3f9d8416e347928e44031cda2f45d8e4193d6f008" },
                { "si", "f47c6e47ca911ca3f31b9937800b2a65677352c7587380786924d8f6c664399a460d27a415a7a857c1baa29fcef30b488f25774da468087b26b2ad534de59007" },
                { "sk", "f7de0660db321ce3f1974bfc4a554c84a627f03415d89ec5bfbdef92f5850f6706582221c9f7402ea329c04879aae4d499a57414bfcebe6a762297d68090017a" },
                { "sl", "7ec567efe34551a4ce8380b86aa691e55805e2306aed6a656434d234c2d3b4bbdbf991db37535ca59e499298d4e976f7456b9e1f19bcf56eddde055765a7c107" },
                { "son", "c10730bed0ee7607ed5492d6d71b929e5208d97fd6501ef6bf0d115ffb9db158bc4025f9f21efeda49058ab4cb960ba7772f6ae28bde7d9f5c9318aa4eabeb9d" },
                { "sq", "c7244c2f1e2df6cb8f5ba9cfc52b6baf0390742c4d968cb2943605d5b8289b255a9963ab142fead0bd49d26f8c3912eee56bfcb8f3f498634b46ebec7e1d1c77" },
                { "sr", "821e807935d1c92f1f4b46b3a3df36ca67595e548523eae9f92de9f1d4b93453651cb1a65f386814923a8fa0f80655896f9bf918c52be41e845cbfae686b1c74" },
                { "sv-SE", "1e798bb59c59beb6f3a43147c7fe8c3240a76a86848f385c709df75d72743033abd0c870b861633d5d07f1b08c48a44e557dacee8005acda0bbe64cdd5b62ab3" },
                { "szl", "765857a449a49cb8b652734922ce5143e085fddf3d9c382964c28b8220f6e76723ef436565c8411802abfaad52ff381aaaa7482851087f75e1876b942a6989e8" },
                { "ta", "73a64f2c617a07055ce0591226e14611e2d409c14253461588cf40efe2d0b3f496618a66f7c3477cf5733dd9adb6f86c7630924dde10840219622abf8bdd392f" },
                { "te", "4245d7f3f6daa9c180c28f3cdbb420bb1e30273224c54a40b4df260f6c3b75d9bdd36fdbd6237c2a7fb06149f16e63414554a9d7315d092f21e48b79ba42701f" },
                { "tg", "2d35fd1dee946a2d0636eed19e5cbf9f69010412d593cf16e54f6964ebd1570dee00c36bc887dc7fd207b20fa35993e2612ad4f090548379e7b35c87713a953a" },
                { "th", "e3130d87d730e64f909d1da884f0bb15f69a15c1d06599f3e3198cc0121d41c42b954de5870531cdea8bd9e7cac982504a4b06985e16ff50f200504d664b75fc" },
                { "tl", "d7fb363e3e870a850ab761a2dbfefdf5d582c1539256c9e9db0d08136a48a123a2f7d028cc02dd7b869e44b98f57d3a074478a0de2287f20dc19c2f9546d062b" },
                { "tr", "2516a2b8f64917e5f0a4c5f37a0c489abba6a26d4e3dd124bf5ffb137b8ee5d2894a8974394ade0b6702d748837d76e683be6cea9afdc242ac3a95ea6b30c172" },
                { "trs", "a7aea82884d013b8af2ae530ba94f9070ef021aa264d2320a69610d5c31d56639e6691e231acff52ce439100a09cc64083ad4b9714d3890915ffae41847c74d4" },
                { "uk", "dcf6f21e191734102a2e6e23d631e0d5aafd0696e22c2b22f46691399e5c92fba85d0475776562ae855ab15e05de2b03b0ce17fb6488d8d9b58d936d91e2dc2d" },
                { "ur", "395fea3b88681b885e879ab463e8e321f39170ba044e751599c090b0f710aa505128545d9375e69d9a4868db248b7c50d1ee238c48e3184da9026e80cf8af1ef" },
                { "uz", "b101e17433b51aecde3ad54446b8185efff2680d2c87153419e5984958ef9196afad22015e75731f8b04abb462b5b32f23217ffea05bc507f640f5d3527a73fb" },
                { "vi", "1c846799c466de7b945fa23bd1f41129868bfb45d5fb234c398ba72920cc4a0fb1e6e0c085a5cfafd157d6a16eb678fc5241a5a21d514ebeca1938137c84a9e0" },
                { "xh", "134477d573e7d82b4ca3b3e1f9dccc19d53a3f92207b461cf372609357a7d4f52df0ba741cc584b2be94e640df204e4f8360c42d809129435fdeee372627cdf8" },
                { "zh-CN", "5569ec11eb94114b2c176774353d167ce0e0679aaace945fb43f9a0d301699f521e575fd42faf574e5104f90866b76dd6eafd1f5c02bf12a9cc8786a57cfedc5" },
                { "zh-TW", "ba84664b1cce619da291154519aa1c6634e2cc1ab508b6b6d029741eab0085b9d6b189b1aa0777476b4ef4987a0a8e0b99b1a1db5f8ef527db751a2d415cbc95" }
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
